using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics;

namespace SharpFlame
{
	public sealed class modMain
	{
		
		public static Timer InitializeDelay;
		public static clsResult InitializeResult = new clsResult("Startup result");
		
		public static frmMain frmMainInstance;
		public static frmGenerator frmGeneratorInstance;
		public static frmOptions frmOptionsInstance;
		
		public static OpenTK.GLControl OpenGL1;
		public static OpenTK.GLControl OpenGL2;
		
		static public void Main()
		{
			
			System.Windows.Forms.Application.EnableVisualStyles();
			
			modProgram.PlatformPathSeparator = System.IO.Path.DirectorySeparatorChar;
			modProgram.SetProgramSubDirs();
			
			modSettings.CreateSettingOptions();
			modControls.CreateControls(); //needed to load key control settings
			clsResult SettingsLoadResult = modSettings.Settings_Load(ref modSettings.InitializeSettings);
			InitializeResult.Add(SettingsLoadResult);
			
			OpenGL1 = new OpenTK.GLControl(new GraphicsMode(new ColorFormat(modSettings.InitializeSettings.MapViewBPP), modSettings.InitializeSettings.MapViewDepth, 0));
			OpenGL2 = new OpenTK.GLControl(new GraphicsMode(new ColorFormat(modSettings.InitializeSettings.TextureViewBPP), modSettings.InitializeSettings.TextureViewDepth, 0));
			
			while (OpenGL1.Context == null || OpenGL2.Context == null)
			{
				//todo, why is this needed
			}
			
			frmMainInstance = new frmMain();
			frmMainInstance.FormClosing += frmMainInstance.frmMain_FormClosing;
			frmMainInstance.DragEnter += frmMainInstance.OpenGL_DragEnter;
			frmMainInstance.DragDrop += frmMainInstance.OpenGL_DragDrop;
			
			try
			{
				modProgram.ProgramIcon = new Icon((new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).Info.DirectoryPath + System.Convert.ToString(modProgram.PlatformPathSeparator) + "flaME.ico");
			}
			catch (Exception ex)
			{
				InitializeResult.WarningAdd(modProgram.ProgramName + " icon is missing: " + ex.Message);
			}
			frmMainInstance.Icon = modProgram.ProgramIcon;
			frmGeneratorInstance.Icon = modProgram.ProgramIcon;
			
			InitializeDelay = new Timer();
			InitializeDelay.Tick += frmMainInstance.Initialize;
			InitializeDelay.Interval = 50;
			InitializeDelay.Enabled = true;
			
			while (!modProgram.ProgramInitializeFinished)
			{
				System.Threading.Thread.Sleep(50);
				Application.DoEvents();
			}
			
			System.Windows.Forms.Application.Run(frmMainInstance);
		}
	}
	
}
