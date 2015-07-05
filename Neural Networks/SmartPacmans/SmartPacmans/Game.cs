using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NeuralProject.Networks;
using NeuralProject.LearningAlgorithms.UnsupervisedLearning.GeneticAlgorithms;

namespace SmartPacmans
{
	// My GUI
	public delegate void Event();
	public class Label
	{
		public static SpriteFont Font;
		public Vector2 Position;
		public string Text;
		public Color TextColor = Color.Azure;
		public Label() { }
		public Label(Vector2 _Position) : this()
		{
			Position = _Position;
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.DrawString(Font, Text, Position, TextColor);
		}
	}
	public class Button
	{
		public static Texture2D Sprite;
		public static SpriteFont Font;
		public bool IsActive = false;
		public Rectangle Bounds;
		public string Text;
		// Events Handler
		public Event OnLeftClick;
		public Event OnRightClick;
		public Button() { }
		public Button(Rectangle _Bounds) : this()
		{
			Bounds = _Bounds;
		}
		public void Update(MouseState _last, MouseState _current)
		{
			if ((Bounds.Contains(_current.X, _current.Y)) && (_last.LeftButton == ButtonState.Released) && (_current.LeftButton == ButtonState.Pressed) && (OnLeftClick != null)) OnLeftClick();
			if ((Bounds.Contains(_current.X, _current.Y)) && (_last.RightButton == ButtonState.Released) && (_current.RightButton == ButtonState.Pressed) && (OnRightClick != null)) OnRightClick();
			if ((!Bounds.Contains(_last.X, _last.Y)) && (Bounds.Contains(_current.X, _current.Y))) IsActive = true; // on mouse over
			if ((Bounds.Contains(_last.X, _last.Y)) && (!Bounds.Contains(_current.X, _current.Y))) IsActive = false; // on mouse out
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.Draw(Sprite, Bounds, (IsActive) ? (Color.DarkOrange) : (Color.Gray));
			_spriteBatch.DrawString(Font, Text, new Vector2(Bounds.Center.X - Text.Length * 5f, Bounds.Center.Y - 10), Color.GreenYellow);
		}
	}
	public class Panel
	{
		public static Texture2D Sprite;
		public Rectangle Bounds;
		public Panel() { }
		public Panel(Rectangle _Bounds) : this()
		{
			Bounds = _Bounds;
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.Draw(Sprite, Bounds, Color.DimGray);
		}
	}
	// GameObjects classes
	public class Pacman
	{
		public static Texture2D Sprite;
		public Vector2 Position;
		public Rectangle Bounds
		{
			get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Sprite.Width, (int)Sprite.Height); }
		}
		public float Rotation = 0f;
		public float Speed = 3f;
		public float LifeTime = 0;
		public int GhostsEaten = 1;
		public float Vitality
		{
			get { return (GhostsEaten * 10000 / LifeTime); }
		}
		public FeedForwardNetwork Network = new FeedForwardNetwork(new BipolarSigmoidFunction(), Game.R.Next(), 4, 7, 1);
		public Label Info = new Label();
		public Pacman()
		{
			Position = new Vector2(Game.R.Next(Game.gameBounds.Width - Sprite.Width), Game.R.Next(Game.gameBounds.Height - Sprite.Height));
		}
		public Pacman(Vector2 _Position)
		{
			Position = _Position;
		}
		public void Update(GameTime _gameTime)
		{
			Position.X += ((float)Math.Cos(Rotation) * Speed);
			Position.Y += ((float)Math.Sin(Rotation) * Speed);
			Position.X = (Position.X % Game.gameBounds.Width + Game.gameBounds.Width) % Game.gameBounds.Width;
			Position.Y = (Position.Y % Game.gameBounds.Height + Game.gameBounds.Height) % Game.gameBounds.Height;
			LifeTime += _gameTime.ElapsedGameTime.Milliseconds;
			Info.Position = new Vector2(Position.X, Position.Y + 10);
			Info.Text = Vitality.ToString();
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, new Vector2(Sprite.Width / 2, Sprite.Height / 2), 1f, SpriteEffects.None, 0f);
			if (Game.ShowInfo) Info.Draw(_spriteBatch);
		}
		public void Think(Vector2 _ghostPosition, Vector2 _pacmanPosition)
		{
			Vector2 toGhost = Vector2.Zero;
			float Alpha = 0f;
			if (_ghostPosition != -Vector2.One)
			{
				toGhost = _ghostPosition - Position;
				float AlphaCosine = ((float)Math.Cos(Rotation) * toGhost.X + (float)Math.Sin(Rotation) * toGhost.Y) / toGhost.Length();
				float AlphaSine = ((float)Math.Cos(Rotation) * toGhost.Y - (float)Math.Sin(Rotation) * toGhost.X) / toGhost.Length();
				Alpha = (AlphaSine < 0) ? (-(float)Math.Acos(AlphaCosine)) : ((float)Math.Acos(AlphaCosine));
			}
			Vector2 toPacman = Vector2.Zero;
			float Beta = 0f;
			if (_pacmanPosition != -Vector2.One)
			{
				toPacman = _pacmanPosition - Position;
				float BetaCosine = ((float)Math.Cos(Rotation) * toPacman.X + (float)Math.Sin(Rotation) * toPacman.Y) / toPacman.Length();
				float BetaSine = ((float)Math.Cos(Rotation) * toPacman.Y - (float)Math.Sin(Rotation) * toPacman.X) / toPacman.Length();
				Beta = (BetaSine < 0) ? (-(float)Math.Acos(BetaCosine)) : ((float)Math.Acos(BetaCosine));
			}
			double[] Answer = Network.Launch(new double[] { toGhost.Length() / 250, Alpha, toPacman.Length() / 250, Beta });
			float Gamma = (float)Answer[0];
			//Rotation = MathHelper.Lerp(Rotation, Rotation + (float)Math.Asin(SineGamma), 0.4f);
			//Rotation = MathHelper.Lerp(Rotation, Rotation + (float)SineGamma, 0.4f);
			Rotation += (float)Gamma;
			Rotation %= (float)MathHelper.TwoPi;
		}
		public void Kill()
		{
			Game.RIPs.Add(Position);
			Game.Pacmans.Remove(this);
		}
	}
	public class RIP
	{
		public static Texture2D Sprite;
		public Vector2 Position;
		public int Duration = 1500;
		public RIP() { }
		public RIP(Vector2 _Position) : this()
		{
			Position = _Position;
		}
		public void Update(GameTime _gameTime)
		{
			Duration = (Duration > _gameTime.ElapsedGameTime.Milliseconds) ? (Duration - _gameTime.ElapsedGameTime.Milliseconds) : (0);
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			if (Duration > 0) _spriteBatch.Draw(Sprite, Position, Color.White);
		}
	}
	public class RIPCenter
	{
		public List<RIP> RIPs = new List<RIP>();
		public RIPCenter() { }
		public void Update(GameTime _gameTime)
		{
			List<RIP> toDelete = new List<RIP>();
			for (int i = 0; i < RIPs.Count; i++) 
			{
				RIPs[i].Update(_gameTime);
				if (RIPs[i].Duration == 0) toDelete.Add(RIPs[i]);
			}
			toDelete.ForEach(rip => RIPs.Remove(rip));
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			for (int i = 0; i < RIPs.Count; i++) RIPs[i].Draw(_spriteBatch);
		}
		public void Add(Vector2 _Position)
		{
			RIPs.Add(new RIP(_Position));
		}
	}
	public class Ghost
	{
		public static Texture2D Sprite;
		public Vector2 Position;
		public Rectangle Bounds
		{
			get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Sprite.Width, (int)Sprite.Height); }
		}
		public float Rotation = 0f;
		public float Speed = 1f;
		public Ghost()
		{
			Position = new Vector2(Game.R.Next(Game.gameBounds.Width - Sprite.Width), Game.R.Next(Game.gameBounds.Height - Sprite.Height));
			Rotation = MathHelper.ToRadians(Game.R.Next(360));
		}
		public Ghost(Vector2 _Position)
		{
			Position = _Position;
			Rotation = MathHelper.ToRadians(Game.R.Next(360));
		}
		public void Update(GameTime gameTime)
		{
			Position.X += ((float)Math.Cos(Rotation) * Speed);
			Position.Y += ((float)Math.Sin(Rotation) * Speed);
			Position.X = (Position.X % Game.gameBounds.Width + Game.gameBounds.Width) % Game.gameBounds.Width;
			Position.Y = (Position.Y % Game.gameBounds.Height + Game.gameBounds.Height) % Game.gameBounds.Height;
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.Draw(Sprite, Position, null, Color.White, 0f, new Vector2(Sprite.Width / 2, Sprite.Height / 2), 1f, SpriteEffects.None, 0f);
		}
	}
	public class NotificationCenter
	{
		public static SpriteFont Font;
		public Label Notification = new Label();
		public static int Duration = 0;
		public NotificationCenter() { }
		public void Update(GameTime _gameTime)
		{
			Duration = (Duration > _gameTime.ElapsedGameTime.Milliseconds) ? (Duration - _gameTime.ElapsedGameTime.Milliseconds) : (0);
		}
		public void Draw(SpriteBatch _spriteBatch)
		{
			if (Duration > 0) Notification.Draw(_spriteBatch);
		}
		public void Push(string _Text)
		{
			Notification = new Label(new Vector2(Game.gameBounds.Center.X - _Text.Length * 4, 10));
			Notification.Text = _Text;
			Duration = 3000;
		}
	}
	public class Cycle
	{
		public Event OnTime;
		public float Duration;
		public float Timer;
		public Cycle() { }
		public Cycle(float _Duration)
		{
			Duration = _Duration;
			Timer = _Duration;
		}
		public void Update(GameTime _gameTime)
		{
			Timer = (Timer > _gameTime.ElapsedGameTime.Milliseconds) ? (Timer - _gameTime.ElapsedGameTime.Milliseconds) : (0);
			if (Timer == 0)
			{
				OnTime();
				Timer = Duration; 
			}
		}
	}
	public class Game : Microsoft.Xna.Framework.Game
	{
		// Standard
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		// Settings
		public static Rectangle windowBounds = new Rectangle(0, 0, 1000, 600);
		public static Rectangle gameBounds = new Rectangle(0, 0, 800, 600);
		public int startPacmansCount = 10;
		public int startGhostsCount = 20;
		// Required
		public static Random R = new Random(Environment.TickCount);
		public static List<Pacman> Pacmans = new List<Pacman>();
		public static List<Ghost> Ghosts = new List<Ghost>();
		public static NotificationCenter Notifications = new NotificationCenter();
		public static RIPCenter RIPs = new RIPCenter();
		public Panel ControlsPanel = new Panel(new Rectangle(gameBounds.Width, 0, windowBounds.Width - gameBounds.Width, windowBounds.Height));
		public Label ControlsLabel = new Label(new Vector2(gameBounds.Width + 55, 20));
		public Button EvolveButton = new Button(new Rectangle(gameBounds.Width + 20, 80, 160, 40));
		public Button CrossAndSelectButton = new Button(new Rectangle(gameBounds.Width + 20, 130, 160, 40));
		public Button MutateButton = new Button(new Rectangle(gameBounds.Width + 20, 180, 160, 40));
		public Button InfoButton = new Button(new Rectangle(gameBounds.Width + 20, 230, 160, 40));
		public Button GhostsStateButton = new Button(new Rectangle(gameBounds.Width + 20, 280, 160, 40));
		public Button GhostsAutoRespawnButton = new Button(new Rectangle(gameBounds.Width + 20, 330, 160, 40));
		public Button LoadButton = new Button(new Rectangle(gameBounds.Width + 20, 380, 160, 40));
		public Button SaveButton = new Button(new Rectangle(gameBounds.Width + 20, 430, 160, 40));
		public Button ExitButton = new Button(new Rectangle(gameBounds.Width + 20, 480, 160, 40));
		public Cycle AutoEvolutionCycle = new Cycle(50000);
		public Cycle AutoCrossingAndSelectionCycle = new Cycle(7000);
		public Cycle AutoMutationCycle = new Cycle(20000);
		public Label EvolutionsLabel = new Label(new Vector2(gameBounds.Width - 100, 10));
		public Label CrossingsLabel = new Label(new Vector2(10, 10));
		public Label MutationsLabel = new Label(new Vector2(10, gameBounds.Height - 35));
		// Modes
		public static bool GhostsAutoRespawn = false;
		public static bool IsAutoEvolution = false;
		public static bool IsAutoCrossingAndSelection = true;
		public static bool IsAutoMutation = true;
		public static bool DynamicGhosts = false;
		public static bool ShowInfo = false;
		public static int Evolutions = 0;
		public static int CrossingsAndSelections = 0;
		public static int Mutations = 0;
		public static string LoadFolder = @"brainsToLoad";
		public static string SaveFolder = @"brainsSaved";
		// I/O
		public KeyboardState lastKeyboardState;
		public KeyboardState currentKeyboardState;
		public MouseState lastMouseState;
		public MouseState currentMouseState;

		// Functions
		public Pacman Crossing(Pacman A, Pacman B)
		{
			Pacman C = new Pacman(new Vector2(R.Next(gameBounds.Width), R.Next(gameBounds.Height)));
			int Range = R.Next(183);
			for (int i = 1; i < A.Network.Layers.Count; i++)
			{
				for (int j = 0; j < A.Network.Layers[i].Neurons.Count; j++)
				{
					for (int k = 0; k < A.Network.Layers[i].Neurons[j].Inputs.Count; k++)
					{
						if (i + j + k >= Range) C.Network.Layers[i].Neurons[j].Inputs[k].Weight = B.Network.Layers[i].Neurons[j].Inputs[k].Weight;
						else C.Network.Layers[i].Neurons[j].Inputs[k].Weight = A.Network.Layers[i].Neurons[j].Inputs[k].Weight;
					}
				}
			}
			return C;
		}
		public void Evolution()
		{
			Pacmans.Sort((A, B) => (A.Vitality.CompareTo(B.Vitality))); // From dumn to clever
			int Half = (Pacmans.Count / 2);
			for (int i = 0; i < Half; i++) Pacmans[i].Kill();
			for (int i = 0; i < Half; i++)
			{
				int First = R.Next(Pacmans.Count);
				int Second = R.Next(Pacmans.Count);
				while (Second == First) Second = R.Next(Pacmans.Count);
				Pacmans.Add(Crossing(Pacmans[First], Pacmans[Second]));
			}
			Evolutions++;
		}
		public void AutoEvolution()
		{
			Evolution();
			Notifications.Push("Auto evolution completed");
		}
		public void CrossingAndSelection()
		{
			Pacmans.Sort((A, B) => (A.Vitality.CompareTo(B.Vitality))); // From dumn to clever
			int Min = R.Next(Pacmans.Count - 2);
			int Average = R.Next(Pacmans.Count - 1);
			while (Average <= Min) Average = R.Next(Pacmans.Count - 1);
			int Max = R.Next(Pacmans.Count);
			while (Max <= Average) Max = R.Next(Pacmans.Count);
			Pacman Temp = Crossing(Pacmans[Max], Pacmans[Average]);
			Pacmans[Min].Kill();
			Pacmans.Add(Temp);
			CrossingsAndSelections++;
		}
		public void AutoCrossingAndSelection()
		{
			CrossingAndSelection();
			Notifications.Push("Auto crossing & selection completed");
		}
		public void Mutation()
		{
			int Index = R.Next(Pacmans.Count);
			//for (int i = 1; i < Pacmans[Index].Network.Layers.Count; i++)
			//{
			//	for (int j = 0; j < Pacmans[Index].Network.Layers[i].Neurons.Count; j++)
			//	{
			//		for (int k = 0; k < Pacmans[Index].Network.Layers[i].Neurons[j].Inputs.Count; k++)
			//		{
			//			if (R.Next(100) == 23)
			//			{
			//				Pacmans[Index].Network.Layers[i].Neurons[j].Inputs[k].Weight += (2 * R.NextDouble() - 1);
			//			}
			//		}
			//	}
			//}
			Nature.Mutate(R.Next(), Pacmans[Index].Network);
			Pacmans[Index].LifeTime = 0;
			Pacmans[Index].GhostsEaten = 1;
			Mutations++;
		}
		public void AutoMutation()
		{
			Mutation();
			Notifications.Push("Auto mutation completed");
		}

		// Event Handlers
		public void EvolveButtonLeftClick()
		{
			Evolution();
			AutoEvolutionCycle.Timer = AutoEvolutionCycle.Duration;
			Notifications.Push("Evolving completed");
		}
		public void EvolveButtonRightClick()
		{
			IsAutoEvolution = !IsAutoEvolution;
			AutoEvolutionCycle.Timer = AutoEvolutionCycle.Duration;
			if (IsAutoEvolution) Notifications.Push("Auto evolution: ON");
			else Notifications.Push("Auto evolution: OFF");
		}
		public void CrossAndSelectButtonLeftClick()
		{
			CrossingAndSelection();
			AutoCrossingAndSelectionCycle.Timer = AutoCrossingAndSelectionCycle.Duration;
			Notifications.Push("Crossing & Selection completed");
		}
		public void CrossAndSelectButtonRightClick()
		{
			IsAutoCrossingAndSelection = !IsAutoCrossingAndSelection;
			AutoCrossingAndSelectionCycle.Timer = AutoCrossingAndSelectionCycle.Duration;
			if (IsAutoCrossingAndSelection) Notifications.Push("Auto crossing & selection: ON");
			else Notifications.Push("Auto crossing & selection: OFF");
		}
		public void MutateButtonLeftClick()
		{
			Mutation();
			AutoMutationCycle.Timer = AutoMutationCycle.Duration;
			Notifications.Push("Mutation completed");
		}
		public void MutateButtonRightClick()
		{
			IsAutoMutation = !IsAutoMutation;
			AutoMutationCycle.Timer = AutoMutationCycle.Duration;
			if (IsAutoMutation) Notifications.Push("Auto mutation: ON");
			else Notifications.Push("Auto mutation: OFF");
		}
		public void InfoButtonLeftClick()
		{
			ShowInfo = !ShowInfo;
			if (ShowInfo) InfoButton.Text = "HIDE INFO";
			else InfoButton.Text = "SHOW INFO";
		}
		public void GhostsStateButtonLeftClick()
		{
			DynamicGhosts = !DynamicGhosts;
			if (DynamicGhosts) 
			{
				GhostsStateButton.Text = "STATIC";
				Notifications.Push("Dynamic ghosts");
			}
			else 
			{
				GhostsStateButton.Text = "DYNAMIC";
				Notifications.Push("Static ghosts");
			}
		}
		public void GhostsAutoRespawnButtonLeftClick()
		{
			GhostsAutoRespawn = !GhostsAutoRespawn;
			if (GhostsAutoRespawn) Notifications.Push("Ghosts auto respawn: ON");
			else Notifications.Push("Ghosts auto respawn: OFF");
		}
		public void LoadButtonLeftClick()
		{
			Pacmans = new List<Pacman>();
			for (int i = 0; i < Directory.GetFiles(LoadFolder, "pacman*.nnw").Length; i++)
			{
				Pacmans.Add(new Pacman(new Vector2(R.Next(gameBounds.Width), R.Next(gameBounds.Height))));
				Pacmans[Pacmans.Count - 1].Network = FeedForwardNetwork.Load(LoadFolder + "\\pacman" + i.ToString() + ".nnw");
			}
			Notifications.Push("Loading completed");
		}
		public void SaveButtonLeftClick()
		{
			if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder);
			for (int i = 0; i < Pacmans.Count; i++)
			{
				Pacmans[i].Network.Save(SaveFolder + "\\pacman" + i.ToString() + ".nnw");
			}
			Notifications.Push("Saving completed");
		}
		public void ExitButtonLeftClick()
		{
			this.Exit();
		}

		public Game()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = windowBounds.Width;
			graphics.PreferredBackBufferHeight = windowBounds.Height;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			this.IsMouseVisible = true;

			// Init cycles
			AutoEvolutionCycle.OnTime = AutoEvolution;
			AutoCrossingAndSelectionCycle.OnTime = AutoCrossingAndSelection;
			AutoMutationCycle.OnTime = AutoMutation;
			// Init labels
			ControlsLabel.Text = "CONTROLS";
			// Initializing buttons
			EvolveButton.Text = "EVOLVE";
			EvolveButton.OnLeftClick = EvolveButtonLeftClick;
			EvolveButton.OnRightClick = EvolveButtonRightClick;
			CrossAndSelectButton.Text = "CROSS & SELECT";
			CrossAndSelectButton.OnLeftClick = CrossAndSelectButtonLeftClick;
			CrossAndSelectButton.OnRightClick = CrossAndSelectButtonRightClick;
			MutateButton.Text = "MUTATE";
			MutateButton.OnLeftClick = MutateButtonLeftClick;
			MutateButton.OnRightClick = MutateButtonRightClick;
			InfoButton.Text = "SHOW INFO";
			InfoButton.OnLeftClick = InfoButtonLeftClick;
			GhostsStateButton.Text = "DYNAMIC";
			GhostsStateButton.OnLeftClick = GhostsStateButtonLeftClick;
			GhostsAutoRespawnButton.Text = "AUTO RESPAWN";
			GhostsAutoRespawnButton.OnLeftClick = GhostsAutoRespawnButtonLeftClick;
			LoadButton.Text = "LOAD";
			LoadButton.OnLeftClick = LoadButtonLeftClick;
			SaveButton.Text = "SAVE";
			SaveButton.OnLeftClick = SaveButtonLeftClick;
			ExitButton.Text = "EXIT";
			ExitButton.OnLeftClick = ExitButtonLeftClick;

			base.Initialize();
		}

		public Texture2D Background;
		public Texture2D Canvas;
		public SpriteFont Font;

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			// Loading clear canvas texture
			Canvas = new Texture2D(GraphicsDevice, 1, 1);
			Canvas.SetData(new Color[] { Color.White });
			Button.Sprite = Canvas;
			Panel.Sprite = Canvas;
			// Loading standard font
			Font = this.Content.Load<SpriteFont>(@"font");
			Label.Font = Font;
			Button.Font = Font;
			// Loading background sprite
			Background = this.Content.Load<Texture2D>(@"background");
			// Loading pacmans sprite
			Pacman.Sprite = this.Content.Load<Texture2D>(@"pacman");
			// Creating pacmans
			for (int i = 0; i < startPacmansCount; i++) Pacmans.Add(new Pacman());
			// Loading rip sprite
			RIP.Sprite = this.Content.Load<Texture2D>(@"rip");
			// Loading ghosts sprite
			Ghost.Sprite = this.Content.Load<Texture2D>(@"ghost3");
			// Creating ghosts
			for (int i = 0; i < startGhostsCount; i++) Ghosts.Add(new Ghost());
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update(GameTime gameTime)
		{
			// I/O
			currentKeyboardState = Keyboard.GetState();
			currentMouseState = Mouse.GetState();
			// Keyboard
			if (currentKeyboardState.IsKeyDown(Keys.Escape)) this.Exit();
			if ((!lastKeyboardState.IsKeyDown(Keys.E)) && (currentKeyboardState.IsKeyDown(Keys.E))) EvolveButtonLeftClick();
			if ((!lastKeyboardState.IsKeyDown(Keys.C)) && (currentKeyboardState.IsKeyDown(Keys.C))) CrossAndSelectButtonLeftClick();
			if ((!lastKeyboardState.IsKeyDown(Keys.M)) && (currentKeyboardState.IsKeyDown(Keys.M))) MutateButtonLeftClick();
			if ((!lastKeyboardState.IsKeyDown(Keys.I)) && (currentKeyboardState.IsKeyDown(Keys.I))) InfoButtonLeftClick();
			if ((!lastKeyboardState.IsKeyDown(Keys.G)) && (currentKeyboardState.IsKeyDown(Keys.G))) GhostsStateButtonLeftClick();
			if ((!lastKeyboardState.IsKeyDown(Keys.R)) && (currentKeyboardState.IsKeyDown(Keys.R))) GhostsAutoRespawnButtonLeftClick();
			// Mouse
			if ((gameBounds.Contains(currentMouseState.X, currentMouseState.Y)) && (lastMouseState.LeftButton == ButtonState.Released) && (currentMouseState.LeftButton == ButtonState.Pressed)) Ghosts.Add(new Ghost(new Vector2(currentMouseState.X, currentMouseState.Y)));
			if ((gameBounds.Contains(currentMouseState.X, currentMouseState.Y)) && (lastMouseState.RightButton == ButtonState.Released) && (currentMouseState.RightButton == ButtonState.Pressed)) Pacmans.Add(new Pacman(new Vector2(currentMouseState.X, currentMouseState.Y)));
			
			// Updating cycles
			if (IsAutoEvolution) AutoEvolutionCycle.Update(gameTime);
			if (IsAutoCrossingAndSelection) AutoCrossingAndSelectionCycle.Update(gameTime);
			if (IsAutoMutation) AutoMutationCycle.Update(gameTime);

			// Updating buttons
			EvolveButton.Update(lastMouseState, currentMouseState);
			CrossAndSelectButton.Update(lastMouseState, currentMouseState);
			MutateButton.Update(lastMouseState, currentMouseState);
			InfoButton.Update(lastMouseState, currentMouseState);
			GhostsStateButton.Update(lastMouseState, currentMouseState);
			GhostsAutoRespawnButton.Update(lastMouseState, currentMouseState);
			LoadButton.Update(lastMouseState, currentMouseState);
			SaveButton.Update(lastMouseState, currentMouseState);
			ExitButton.Update(lastMouseState, currentMouseState);

			// Updating
			List<Ghost> ghostsToDelete = new List<Ghost>();
			foreach (Pacman current in Pacmans)
			{
				Vector2 nearestGhostPosition = -Vector2.One;
				Rectangle currentRectangle = current.Bounds;
				foreach (Ghost ghost in Ghosts)
				{
					Vector2 toGhost = ghost.Position - current.Position;
					float AlphaCosine = ((float)Math.Cos(current.Rotation) * toGhost.X + (float)Math.Sin(current.Rotation) * toGhost.Y) / toGhost.Length();
					//float AlphaSine = ((float)Math.Cos(current.Rotation) * toGhost.Y - (float)Math.Sin(current.Rotation) * toGhost.X) / toGhost.Length();
					if ((AlphaCosine >= 0) && ((nearestGhostPosition == Vector2.Zero) || ((nearestGhostPosition != Vector2.Zero) && (Vector2.DistanceSquared(current.Position, ghost.Position) < Vector2.DistanceSquared(current.Position, nearestGhostPosition))))) nearestGhostPosition = ghost.Position;
					if ((!ghostsToDelete.Contains(ghost)) && (currentRectangle.Intersects(ghost.Bounds)))
					{
						ghostsToDelete.Add(ghost);
						current.GhostsEaten += 5;
					}
				}
				Vector2 nearestPacmanPosition = -Vector2.One;
				foreach (Pacman pacman in Pacmans)
				{
					Vector2 toPacman = pacman.Position - current.Position;
					float BetaCosine = ((float)Math.Cos(current.Rotation) * toPacman.X + (float)Math.Sin(current.Rotation) * toPacman.Y) / toPacman.Length();
					//float BetaSine = ((float)Math.Cos(current.Rotation) * toPacman.Y - (float)Math.Sin(current.Rotation) * toPacman.X) / toPacman.Length();
					if ((current != pacman) && (BetaCosine >= 0) && ((nearestPacmanPosition == Vector2.Zero) || ((nearestPacmanPosition != Vector2.Zero) && (Vector2.DistanceSquared(current.Position, pacman.Position) < Vector2.DistanceSquared(current.Position, nearestPacmanPosition))))) nearestPacmanPosition = pacman.Position;
				}
				current.Think(nearestGhostPosition, nearestPacmanPosition);
				current.Update(gameTime);
			}
			for (int i = 0; i < ghostsToDelete.Count; i++)
			{
				Ghosts.Remove(ghostsToDelete[i]);
				if (GhostsAutoRespawn) Ghosts.Add(new Ghost());
			}
			if (DynamicGhosts) foreach (Ghost ghost in Ghosts) ghost.Update(gameTime);

			// Updating RIPs
			RIPs.Update(gameTime);

			// Updating notifications
			Notifications.Update(gameTime);

			// Updateing labels
			EvolutionsLabel.Text = "Evolution: " + Evolutions.ToString();
			CrossingsLabel.Text = "Crossings: " + CrossingsAndSelections.ToString();
			MutationsLabel.Text = "Mutations: " + Mutations.ToString();

			// Updating I/O states
			lastKeyboardState = currentKeyboardState;
			lastMouseState = currentMouseState;
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			//GraphicsDevice.Clear(Color.FromNonPremultiplied(115,150,78, 255));
			spriteBatch.Begin();
			// Drawing background
			spriteBatch.Draw(Background, new Rectangle(0, 0, gameBounds.Width, gameBounds.Height), Color.FromNonPremultiplied(100, 255, 255, 255));
			for (int i = 0; i < Pacmans.Count; i++) Pacmans[i].Draw(spriteBatch);
			for (int i = 0; i < Ghosts.Count; i++) Ghosts[i].Draw(spriteBatch);
			// Drawing RIPs
			RIPs.Draw(spriteBatch);
			// Drawing Notifications
			Notifications.Draw(spriteBatch);
			// Drawing Controls panel
			ControlsPanel.Draw(spriteBatch);
			// Drawing labels
			ControlsLabel.Draw(spriteBatch);
			EvolutionsLabel.Draw(spriteBatch);
			CrossingsLabel.Draw(spriteBatch);
			MutationsLabel.Draw(spriteBatch);
			// Drawing buttons
			EvolveButton.Draw(spriteBatch);
			CrossAndSelectButton.Draw(spriteBatch);
			MutateButton.Draw(spriteBatch);
			InfoButton.Draw(spriteBatch);
			GhostsStateButton.Draw(spriteBatch);
			GhostsAutoRespawnButton.Draw(spriteBatch);
			LoadButton.Draw(spriteBatch);
			SaveButton.Draw(spriteBatch);
			ExitButton.Draw(spriteBatch);
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
