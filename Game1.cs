using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Fool
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Deck deck = new Deck();

        public List<Player> Players = new List<Player>();
        public List<string> PlayersNames = new List<string>();
        public string[] TemplatePlayerNames = ["John","Bob","Snippy","Dzhake","Albert","Andrew","Bill","Bruce","Charles III",
            "Clark","RIP Clyde","Edwin","Everest","Guy","Henry Stickmin","Harry Potter","Patrick"];

        public Card.Suits TrumpSuit;
        public Card TrumpCard = new Card();

        public int AttackingPlayer;
        public int DefencingPlayer;

        public List<Card> Table = new List<Card>();

        public int Winner = -1;
        public int playersAmount;

        public Texture2D texture;

        public Game1(int playersAmount)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            this.playersAmount = playersAmount;
        }

        protected override void Initialize()
        {
            deck = new Deck();
            deck.Shuffle();
            Players = new List<Player>();


            // Create players
            for (int j = 0; j < playersAmount; j++)
            {
                Player p = new Player(j == 0);
                p.DrawCards();
                Players.Add(p);
                PlayersNames.Add(Utils.RandomArrayElement(TemplatePlayerNames));
            }

            //Init trump suit
            TrumpCard = deck.Draw();
            TrumpSuit = TrumpCard.Suit;
            //ColorConsole.WriteLine($"Trump suit is <{Enum.GetName(TrumpSuit)}>, because trump card is {TrumpCard.ToColoredString()}");
            deck.Cards.Add(TrumpCard);

            // Pick attacking and defencing player
            AttackingPlayer = Utils.RandomListIndex(Players);
            DefencingPlayer = (AttackingPlayer + 1) % Players.Count;
            //ColorConsole.WriteLine($"First <player> is <{PlayersName(AttackingPlayer)}>");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("Sprites/8BitDeck");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
