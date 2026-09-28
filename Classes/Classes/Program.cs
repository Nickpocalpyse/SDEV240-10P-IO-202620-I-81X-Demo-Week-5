
static class Extension
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> List)
    {
        int counter = List.Count;
        while (counter > 1)
        {
            counter--;
            int replacement = rng.Next(counter + 1);
            T value = List[replacement];
            List[replacement] = List[counter];
            List[counter] = value;
        }
    }
}

class GameAction
{
    protected Player player;
    protected Deck deck;

    public GameAction(Player player, Deck deck)
    {
        this.player = player;
        this.deck = deck;

    }
    public virtual void Execute()
    {
    }
}

class DrawAction : GameAction
{
    private DiscardPile discardPile;

    public DrawAction(Player player, Deck deck, DiscardPile discardPile) : base(player, deck)
    {
        this.discardPile = discardPile;
    }

    public override void Execute()
    {
        string card = deck.DrawCard();

        if (card != null)
        {
            Console.WriteLine($"{player.GetName()} drew the card: {card}");
            Console.Write("Enter the position of the card to replace (1-6): ");
            int position = Convert.ToInt32(Console.ReadLine());
            position--;
            string replacedCard = player.ReplaceCard(position, card);
            if (replacedCard != null)
            {
                discardPile.Addcard(replacedCard);
                Console.WriteLine($"Replaced {replacedCard} with {card}");
            }
            else
            {
                Console.WriteLine("Invalid position.");
                deck.CardDeck.Insert(0, card);
            }
        }
        else
        {
            Console.WriteLine("The deck is empty.");
        }
    }
}

class DiscardAction : GameAction
{
    private DiscardPile discardPile;

    public DiscardAction(Player player, Deck deck, DiscardPile discardPile) : base(player, deck)
    {
        this.discardPile = discardPile;
    }

    public override void Execute()
    {
        Console.WriteLine($"{player.GetName()}'s cards:");
        player.ShowCards();
        string card = discardPile.TakeTopCard();
        if (card == null)
        {
            Console.WriteLine("The discard pile is empty.");
            return;
        }
        Console.WriteLine($"You took {card} from the discard pile.");
        Console.Write("Enter the position of the card to replace (1-6): ");
        int position = Convert.ToInt32(Console.ReadLine()) - 1;
        string replacedCard = player.ReplaceCard(position, card);
        if (replacedCard != null)
        {
            discardPile.Addcard(replacedCard);
            Console.WriteLine($"Replaced {replacedCard} with {card}");
        }
        else
        {
            discardPile.Addcard(card);
            Console.WriteLine("Invalid position.");
        }
        Console.WriteLine();
        Console.WriteLine("After the move:");
        player.ShowCards();
        Console.WriteLine();
        discardPile.ShowPile();
    }
}

class Player
    {
        private List<string> Cards;
        private List<bool> FaceUp;
        private string PlayerName;

        public Player(string PlayerName)
        {
            this.PlayerName = PlayerName;
            Cards = new List<string>();
            FaceUp = new List<bool>();
        }

        public string GetName()
        {
            return PlayerName;
        }

        public void AddCard(string card)
        {
            Cards.Add(card);
            FaceUp.Add(false);
        }

        public void ShowCards()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (FaceUp[i])
                {
                    Console.WriteLine($"{i + 1}: {Cards[i]}");
                }
                else
                {
                    Console.WriteLine($"{i + 1}: [Hidden]");
                }
            }
        }

        public string RemoveCard(int position)
        {
            if (position < 0 || position >= Cards.Count)
            {
                return null;
            }
            string card = Cards[position];
            Cards.RemoveAt(position);
            FaceUp.RemoveAt(position);
            return card;
        }

        public string ReplaceCard(int position, string ReplacementCard)
        {
            if (position < 0 || position >= Cards.Count)
            {
                return null;
            }
            string ReplacedCard = Cards[position];
            Cards[position] = ReplacementCard;
            FaceUp[position] = true;
            return ReplacedCard;
        }

        public void FirstTurn(int position)
        {
            if (position >= 0 && position < Cards.Count)
            {
                FaceUp[position] = true;
            }
        }

        public int CalculateScore(Dictionary<string, int> Points)
        {
            int score = 0;
            int[] PointsArray = new int[6];
            foreach (string card in Cards)
            {
                PointsArray[Cards.IndexOf(card)] = Points[card];
            }
            for (int i = 0; i < 3; i++)
                {
                    if (PointsArray[i] == PointsArray[i + 3])
                        {
                            score += PointsArray[i];
                        }
                    else
                        {
                            score += PointsArray[i] + PointsArray[i + 3];
                        }
                }
            return score;
        }
}

class Deck
    {
        public List<string> CardDeck = new List<string> 
            {"A♣", "2♣", "3♣", "4♣", "5♣", "6♣", "7♣", "8♣", "9♣", "10♣", "J♣", "Q♣", "K♣",
            "A♥", "2♥", "3♥", "4♥", "5♥", "6♥", "7♥", "8♥", "9♥", "10♥", "J♥", "Q♥", "K♥",
            "A♦", "2♦", "3♦", "4♦", "5♦", "6♦", "7♦", "8♦", "9♦", "10♦", "J♦", "Q♦", "K♦", 
            "A♠", "2♠", "3♠", "4♠", "5♠", "6♠", "7♠", "8♠", "9♠", "10♠", "J♠", "Q♠", "K♠",
            "*", "*"};
        public List<string> BackUpDeck = new List<string>();
        public Dictionary<string, int> Points = new Dictionary<string, int> {
            {"A♣", 1}, {"A♥", 1}, {"A♦", 1}, {"A♠", 1},
            {"2♣", 2}, {"2♥", 2}, {"2♦", 2}, {"2♠", 2},
            {"3♣", 3}, {"3♥", 3}, {"3♦", 3}, {"3♠", 3},
            {"4♣", 4}, {"4♥", 4}, {"4♦", 4}, {"4♠", 4},
            {"5♣", 5}, {"5♥", 5}, {"5♦", 5}, {"5♠", 5},
            {"6♣", 6}, {"6♥", 6}, {"6♦", 6}, {"6♠", 6},
            {"7♣", 7}, {"7♥", 7}, {"7♦", 7}, {"7♠", 7},
            {"8♣", 8}, {"8♥", 8}, {"8♦", 8}, {"8♠", 8},
            {"9♣", 9}, {"9♥", 9}, {"9♦", 9}, {"9♠", 9},
            {"10♣", 10}, {"10♥", 10}, {"10♦", 10}, {"10♠", 10},
            {"J♣", 10}, {"J♥", 10}, {"J♦", 10}, {"J♠", 10},
            {"Q♣", 10}, {"Q♥", 10}, {"Q♦", 10}, {"Q♠", 10},
            {"K♣", 0}, {"K♥", 0}, {"K♦", 0}, {"K♠", 0},
            {"*", -2}};

        public void Shuffling()
        {
            CardDeck.Shuffle();
        }

        public string DrawCard()
        {
            if (CardDeck.Count == 0)
            {
                return null;
            }
            string card = CardDeck[0];
            CardDeck.RemoveAt(0);
            return card;
        }
}

class DiscardPile
{
    private List<string> Cards = new List<string>();

    public void Addcard(string card)
    {
        Cards.Add(card);
    }

    public string TakeTopCard()
    {
        if (Cards.Count == 0)
        {
            return null;
        }
        string card = Cards[Cards.Count - 1];
        Cards.RemoveAt(Cards.Count - 1);
        return card;
    }

    public string GetTopCard()
    {
        if (Cards.Count == 0)
        {
            return null;
        }
        return Cards[Cards.Count - 1];
    }

    public void ShowPile()
    {
        Console.WriteLine("Discard Pile:");
        foreach (string card in Cards)
        {
            Console.WriteLine(card);
        }
    }
}

class GamePlay
    {

    static void DoFirstTurn(Player player)
    {
        Console.WriteLine();
        Console.WriteLine($"{player.GetName()}'s first turn:");
        player.ShowCards();
        Console.Write("Choose the first card to reveal (1-6): ");
        int first = Convert.ToInt32(Console.ReadLine()) - 1;
        player.FirstTurn(first);
        Console.WriteLine();
        player.ShowCards();
        Console.Write("Choose the second card to reveal (1-6): ");
        int second = Convert.ToInt32(Console.ReadLine()) - 1;
        player.FirstTurn(second);
        Console.WriteLine();
        Console.WriteLine("Your starting hand:");
        player.ShowCards();
    }

    static void Main()
    {
        Deck deck = new Deck();
        deck.Shuffling();
        Player player1 = new Player("Player 1");
        Player player2 = new Player("Player 2");
        DiscardPile discardPile = new DiscardPile();
        for (int i = 0; i < 6; i++)
        {
            player1.AddCard(deck.DrawCard());
            player2.AddCard(deck.DrawCard());
        }
        Console.WriteLine(player1.GetName());
        player1.ShowCards();
        Console.WriteLine();
        Console.WriteLine(player2.GetName());
        player2.ShowCards();
        Console.WriteLine();
        DoFirstTurn(player1);
        DoFirstTurn(player2);
        DrawAction draw = new DrawAction(player1, deck, discardPile);
        draw.Execute();
        Console.WriteLine();
        Console.WriteLine("After Player 1's turn:");
        player1.ShowCards();
        DiscardAction discard = new DiscardAction(player2, deck, discardPile);
        discard.Execute();
        player2.ShowCards();
        int score1 = player1.CalculateScore(deck.Points);
        int score2 = player2.CalculateScore(deck.Points);
        Console.WriteLine();
        Console.WriteLine($"{player1.GetName()} score: {score1}");
        Console.WriteLine($"{player2.GetName()} score: {score2}");
    }
}
