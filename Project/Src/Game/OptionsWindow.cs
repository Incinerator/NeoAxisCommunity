// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Engine;
using Engine.FileSystem;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using Engine.MapSystem;
using Engine.Utils;
using Engine.SoundSystem;
using ProjectCommon;
using ProjectEntities;
///<summary>
///Incin -- This Source is intended for accessing any and all devices that can be used for input to the game engine , phones , TV remotes...
/// anything that can be bound to a pc. Pulling from the InputDevice of the object
///</summary>
namespace Game
{
	/// <summary>
	/// Defines a window of options.
	/// </summary>
	public class OptionsWindow : Control
	{
		//Enum device  Types installed on system
		enum Devices
		{
			//System,
			GetServices,
			//System InputDevices 
			Keyboard, //what type of keyboard? phone? keyboard? or default values
			Mouse, //name by name of device or default values
			Joystick, //named by name of device or default values
			Joystick_Xbox360, //name by name of device or default values
			Joystick_WII,
			Joystick_Playstation,
			Custom, //add customized Controls.. any custom Controls designed
			Custom_Audio, //named by device or device defaults example (zoom device to control guitar inputs)
			RemoteControl, //A controleer for TV.. bluetoothed?

			All_Devices // Index ... No added devices? wtf
		}

		static int lastPageIndex;
		Control window;	
		Button[] pageButtons = new Button[ 5 ];
		TabControl tabControl;

		ComboBox comboBoxResolution;
		ComboBox comboBoxInputDevices;
		CheckBox checkBoxDepthBufferAccess;
		ComboBox comboBoxAntialiasing;

		//Incin -- Need class access to these items
		private ComboBox cmbBoxDevice; //need local access to this
		private ListBox controlsList = null; //need local access for this
		private JoystickAxisFilters axisfilterselection = JoystickAxisFilters.DEADZONE; //this is used to select filter axis of each 
		private Button axisfilterbutton;
		private Button btnAddBinding; //for adding commands to binding window
		///////////////////////////////////////////

		class ComboBoxItem
		{
			string identifier;
			string displayName;

			public ComboBoxItem( string identifier, string displayName )
			{
				this.identifier = identifier;
				this.displayName = displayName;
			}

			public string Identifier
			{
				get { return identifier; }
			}

			public string DisplayName
			{
				get { return displayName; }
			}

			public override string ToString()
			{
				return displayName;
			}
		}

		///////////////////////////////////////////

		public class ShadowTechniqueItem
		{
			ShadowTechniques technique;
			string text;

			public ShadowTechniqueItem( ShadowTechniques technique, string text )
			{
				this.technique = technique;
				this.text = text;
			}

			public ShadowTechniques Technique
			{
				get { return technique; }
			}

			public override string ToString()
			{
				return text;
			}
		}

		///////////////////////////////////////////

		protected override void OnAttach()
		{
			base.OnAttach();

			ComboBox comboBox;
			ScrollBar scrollBar;
			CheckBox checkBox;
			TextBox textBox;

			window = ControlDeclarationManager.Instance.CreateControl( "Gui\\OptionsWindow.gui" );
			Controls.Add( window );

			tabControl = (TabControl)window.Controls[ "TabControl" ];

			BackColor = new ColorValue( 0, 0, 0, .5f );
			MouseCover = true;

			//load Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			TextBlock rendererBlock = null;
			if( engineConfigBlock != null )
				rendererBlock = engineConfigBlock.FindChild( "Renderer" );

			//page buttons
			pageButtons[ 0 ] = (Button)window.Controls[ "ButtonVideo" ];
			pageButtons[ 1 ] = (Button)window.Controls[ "ButtonShadows" ];
			pageButtons[ 2 ] = (Button)window.Controls[ "ButtonSound" ];
			pageButtons[ 3 ] = (Button)window.Controls[ "ButtonControls" ];
			pageButtons[ 4 ] = (Button)window.Controls[ "ButtonLanguage" ];
			foreach( Button pageButton in pageButtons )
				pageButton.Click += new Button.ClickDelegate( pageButton_Click );

			//Close button
			( (Button)window.Controls[ "Close" ] ).Click += delegate( Button sender )
			{
				SetShouldDetach();
			};

			//pageVideo
			{
				Control pageVideo = tabControl.Controls[ "Video" ];

				Vec2I currentMode = EngineApp.Instance.VideoMode;

				//screenResolutionComboBox
				comboBox = (ComboBox)pageVideo.Controls[ "ScreenResolution" ];
				comboBox.Enable = !EngineApp.Instance.MultiMonitorMode;
				comboBoxResolution = comboBox;

				if( EngineApp.Instance.MultiMonitorMode )
				{
					comboBox.Items.Add( string.Format( "{0}x{1} (multi-monitor)", currentMode.X,
						currentMode.Y ) );
					comboBox.SelectedIndex = 0;
				}
				else
				{
					foreach( Vec2I mode in DisplaySettings.VideoModes )
					{
						if( mode.X < 640 )
							continue;

						comboBox.Items.Add( string.Format( "{0}x{1}", mode.X, mode.Y ) );

						if( mode == currentMode )
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
					}

					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						ChangeVideoMode();
					};
				}

				//gamma
				scrollBar = (ScrollBar)pageVideo.Controls[ "Gamma" ];
				scrollBar.Value = GameEngineApp._Gamma;
				scrollBar.Enable = true;
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					float value = float.Parse( sender.Value.ToString( "F1" ) );
					GameEngineApp._Gamma = value;
					pageVideo.Controls[ "GammaValue" ].Text = value.ToString( "F1" );
				};
				pageVideo.Controls[ "GammaValue" ].Text = GameEngineApp._Gamma.ToString( "F1" );

				//MaterialScheme
				{
					comboBox = (ComboBox)pageVideo.Controls[ "MaterialScheme" ];
					foreach( MaterialSchemes materialScheme in
						Enum.GetValues( typeof( MaterialSchemes ) ) )
					{
						comboBox.Items.Add( materialScheme.ToString() );

						if( GameEngineApp.MaterialScheme == materialScheme )
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
					}
					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						if( sender.SelectedIndex != -1 )
							GameEngineApp.MaterialScheme = (MaterialSchemes)sender.SelectedIndex;
					};
				}

				//fullScreen
				checkBox = (CheckBox)pageVideo.Controls[ "FullScreen" ];
				checkBox.Enable = !EngineApp.Instance.MultiMonitorMode;
				checkBox.Checked = EngineApp.Instance.FullScreen;
				checkBox.CheckedChange += delegate( CheckBox sender )
				{
					EngineApp.Instance.FullScreen = sender.Checked;
				};

				//RenderTechnique
				{
					comboBox = (ComboBox)pageVideo.Controls[ "RenderTechnique" ];
					comboBox.Items.Add( new ComboBoxItem( "RecommendedSetting", Translate( "Recommended setting" ) ) );
					comboBox.Items.Add( new ComboBoxItem( "Standard", Translate( "Low Dynamic Range (Standard)" ) ) );
					comboBox.Items.Add( new ComboBoxItem( "HDR", Translate( "High Dynamic Range (HDR)" ) ) );

					string renderTechnique = "";
					if( rendererBlock != null && rendererBlock.IsAttributeExist( "renderTechnique" ) )
						renderTechnique = rendererBlock.GetAttribute( "renderTechnique" );

					for( int n = 0; n < comboBox.Items.Count; n++ )
					{
						ComboBoxItem item = (ComboBoxItem)comboBox.Items[ n ];
						if( item.Identifier == renderTechnique )
							comboBox.SelectedIndex = n;
					}
					if( comboBox.SelectedIndex == -1 )
						comboBox.SelectedIndex = 0;

					comboBox.SelectedIndexChange += comboBoxRenderTechnique_SelectedIndexChange;
				}

				//Filtering
				{
					comboBox = (ComboBox)pageVideo.Controls[ "Filtering" ];

					Type enumType = typeof( RendererWorld.FilteringModes );
					LocalizedEnumConverter enumConverter = new LocalizedEnumConverter( enumType );

					RendererWorld.FilteringModes filtering = RendererWorld.FilteringModes.RecommendedSetting;
					//get value from Engine.config.
					if( rendererBlock != null && rendererBlock.IsAttributeExist( "filtering" ) )
					{
						try
						{
							filtering = (RendererWorld.FilteringModes)Enum.Parse( enumType, rendererBlock.GetAttribute( "filtering" ) );
						}
						catch { }
					}

					RendererWorld.FilteringModes[] values = (RendererWorld.FilteringModes[])Enum.GetValues( enumType );
					for( int n = 0; n < values.Length; n++ )
					{
						RendererWorld.FilteringModes value = values[ n ];
						string valueStr = enumConverter.ConvertToString( value );
						comboBox.Items.Add( new ComboBoxItem( value.ToString(), Translate( valueStr ) ) );
						if( filtering == value )
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
					}
					if( comboBox.SelectedIndex == -1 )
						comboBox.SelectedIndex = 0;

					comboBox.SelectedIndexChange += comboBoxFiltering_SelectedIndexChange;
				}

				//DepthBufferAccess
				{
					checkBox = (CheckBox)pageVideo.Controls[ "DepthBufferAccess" ];
					checkBoxDepthBufferAccess = checkBox;

					bool depthBufferAccess = true;
					//get value from Engine.config.
					if( rendererBlock != null && rendererBlock.IsAttributeExist( "depthBufferAccess" ) )
						depthBufferAccess = bool.Parse( rendererBlock.GetAttribute( "depthBufferAccess" ) );
					checkBox.Checked = depthBufferAccess;

					checkBox.CheckedChange += checkBoxDepthBufferAccess_CheckedChange;
				}

				//FSAA
				{
					comboBox = (ComboBox)pageVideo.Controls[ "FSAA" ];
					comboBoxAntialiasing = comboBox;

					UpdateComboBoxAntialiasing();

					string fullSceneAntialiasing = "";
					if( rendererBlock != null && rendererBlock.IsAttributeExist( "fullSceneAntialiasing" ) )
						fullSceneAntialiasing = rendererBlock.GetAttribute( "fullSceneAntialiasing" );
					for( int n = 0; n < comboBoxAntialiasing.Items.Count; n++ )
					{
						ComboBoxItem item = (ComboBoxItem)comboBoxAntialiasing.Items[ n ];
						if( item.Identifier == fullSceneAntialiasing )
							comboBoxAntialiasing.SelectedIndex = n;
					}

					comboBoxAntialiasing.SelectedIndexChange += comboBoxAntialiasing_SelectedIndexChange;
				}

				//VerticalSync
				{
					checkBox = (CheckBox)pageVideo.Controls[ "VerticalSync" ];

					bool verticalSync = RendererWorld.InitializationOptions.VerticalSync;
					//get value from Engine.config.
					if( rendererBlock != null && rendererBlock.IsAttributeExist( "verticalSync" ) )
						verticalSync = bool.Parse( rendererBlock.GetAttribute( "verticalSync" ) );
					checkBox.Checked = verticalSync;

					checkBox.CheckedChange += checkBoxVerticalSync_CheckedChange;
				}

				//VideoRestart
				{
					Button button = (Button)pageVideo.Controls[ "VideoRestart" ];
					button.Click += buttonVideoRestart_Click;
				}

				//waterReflectionLevel
				comboBox = (ComboBox)pageVideo.Controls[ "WaterReflectionLevel" ];
				foreach( WaterPlane.ReflectionLevels level in Enum.GetValues(
					typeof( WaterPlane.ReflectionLevels ) ) )
				{
					comboBox.Items.Add( level );
					if( GameEngineApp.WaterReflectionLevel == level )
						comboBox.SelectedIndex = comboBox.Items.Count - 1;
				}
				comboBox.SelectedIndexChange += delegate( ComboBox sender )
				{
					GameEngineApp.WaterReflectionLevel = (WaterPlane.ReflectionLevels)sender.SelectedItem;
				};

				//showDecorativeObjects
				checkBox = (CheckBox)pageVideo.Controls[ "ShowDecorativeObjects" ];
				checkBox.Checked = GameEngineApp.ShowDecorativeObjects;
				checkBox.CheckedChange += delegate( CheckBox sender )
				{
					GameEngineApp.ShowDecorativeObjects = sender.Checked;
				};

				//showSystemCursorCheckBox
				checkBox = (CheckBox)pageVideo.Controls[ "ShowSystemCursor" ];
				checkBox.Checked = GameEngineApp._ShowSystemCursor;
				checkBox.CheckedChange += delegate( CheckBox sender )
				{
					GameEngineApp._ShowSystemCursor = sender.Checked;
					sender.Checked = GameEngineApp._ShowSystemCursor;
				};

				//showFPSCheckBox
				checkBox = (CheckBox)pageVideo.Controls[ "ShowFPS" ];
				checkBox.Checked = GameEngineApp._DrawFPS;
				checkBox.CheckedChange += delegate( CheckBox sender )
				{
					GameEngineApp._DrawFPS = sender.Checked;
					sender.Checked = GameEngineApp._DrawFPS;
				};
			}

			//pageShadows
			{
				Control pageShadows = tabControl.Controls[ "Shadows" ];

				//ShadowTechnique
				{
					comboBox = (ComboBox)pageShadows.Controls[ "ShadowTechnique" ];

					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.None, "None" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.ShadowmapLow, "Shadowmap Low" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.ShadowmapMedium, "Shadowmap Medium" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.ShadowmapHigh, "Shadowmap High" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.ShadowmapLowPSSM, "PSSMx3 Low" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.ShadowmapMediumPSSM, "PSSMx3 Medium" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.ShadowmapHighPSSM, "PSSMx3 High" ) );
					comboBox.Items.Add( new ShadowTechniqueItem( ShadowTechniques.Stencil, "Stencil" ) );

					for( int n = 0; n < comboBox.Items.Count; n++ )
					{
						ShadowTechniqueItem item = (ShadowTechniqueItem)comboBox.Items[ n ];
						if( item.Technique == GameEngineApp.ShadowTechnique )
							comboBox.SelectedIndex = n;
					}

					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						if( sender.SelectedIndex != -1 )
						{
							ShadowTechniqueItem item = (ShadowTechniqueItem)sender.SelectedItem;
							GameEngineApp.ShadowTechnique = item.Technique;
						}
						UpdateShadowControlsEnable();
					};
					UpdateShadowControlsEnable();
				}

				//ShadowUseMapSettings
				{
					checkBox = (CheckBox)pageShadows.Controls[ "ShadowUseMapSettings" ];
					checkBox.Checked = GameEngineApp.ShadowUseMapSettings;
					checkBox.CheckedChange += delegate( CheckBox sender )
					{
						GameEngineApp.ShadowUseMapSettings = sender.Checked;
						if( sender.Checked && Map.Instance != null )
						{
							GameEngineApp.ShadowPSSMSplitFactors = Map.Instance.InitialShadowPSSMSplitFactors;
							GameEngineApp.ShadowFarDistance = Map.Instance.InitialShadowFarDistance;
							GameEngineApp.ShadowColor = Map.Instance.InitialShadowColor;
						}

						UpdateShadowControlsEnable();

						if( sender.Checked )
						{
							( (ScrollBar)pageShadows.Controls[ "ShadowFarDistance" ] ).Value =
								GameEngineApp.ShadowFarDistance;

							pageShadows.Controls[ "ShadowFarDistanceValue" ].Text =
								( (int)GameEngineApp.ShadowFarDistance ).ToString();

							ColorValue color = GameEngineApp.ShadowColor;
							( (ScrollBar)pageShadows.Controls[ "ShadowColor" ] ).Value =
								( color.Red + color.Green + color.Blue ) / 3;
						}
					};
				}

				//ShadowPSSMSplitFactor1
				scrollBar = (ScrollBar)pageShadows.Controls[ "ShadowPSSMSplitFactor1" ];
				scrollBar.Value = GameEngineApp.ShadowPSSMSplitFactors[ 0 ];
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					GameEngineApp.ShadowPSSMSplitFactors = new Vec2(
						sender.Value, GameEngineApp.ShadowPSSMSplitFactors[ 1 ] );
					pageShadows.Controls[ "ShadowPSSMSplitFactor1Value" ].Text =
						( GameEngineApp.ShadowPSSMSplitFactors[ 0 ].ToString( "F2" ) ).ToString();
				};
				pageShadows.Controls[ "ShadowPSSMSplitFactor1Value" ].Text =
					( GameEngineApp.ShadowPSSMSplitFactors[ 0 ].ToString( "F2" ) ).ToString();

				//ShadowPSSMSplitFactor2
				scrollBar = (ScrollBar)pageShadows.Controls[ "ShadowPSSMSplitFactor2" ];
				scrollBar.Value = GameEngineApp.ShadowPSSMSplitFactors[ 1 ];
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					GameEngineApp.ShadowPSSMSplitFactors = new Vec2(
						GameEngineApp.ShadowPSSMSplitFactors[ 0 ], sender.Value );
					pageShadows.Controls[ "ShadowPSSMSplitFactor2Value" ].Text =
						( GameEngineApp.ShadowPSSMSplitFactors[ 1 ].ToString( "F2" ) ).ToString();
				};
				pageShadows.Controls[ "ShadowPSSMSplitFactor2Value" ].Text =
					( GameEngineApp.ShadowPSSMSplitFactors[ 1 ].ToString( "F2" ) ).ToString();

				//ShadowFarDistance
				scrollBar = (ScrollBar)pageShadows.Controls[ "ShadowFarDistance" ];
				scrollBar.Value = GameEngineApp.ShadowFarDistance;
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					GameEngineApp.ShadowFarDistance = sender.Value;
					pageShadows.Controls[ "ShadowFarDistanceValue" ].Text =
						( (int)GameEngineApp.ShadowFarDistance ).ToString();
				};
				pageShadows.Controls[ "ShadowFarDistanceValue" ].Text =
					( (int)GameEngineApp.ShadowFarDistance ).ToString();

				//ShadowColor
				scrollBar = (ScrollBar)pageShadows.Controls[ "ShadowColor" ];
				scrollBar.Value = ( GameEngineApp.ShadowColor.Red + GameEngineApp.ShadowColor.Green +
					GameEngineApp.ShadowColor.Blue ) / 3;
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					float color = sender.Value;
					GameEngineApp.ShadowColor = new ColorValue( color, color, color, color );
				};

				//ShadowDirectionalLightTextureSize
				{
					comboBox = (ComboBox)pageShadows.Controls[ "ShadowDirectionalLightTextureSize" ];
					for( int value = 256, index = 0; value <= 8192; value *= 2, index++ )
					{
						comboBox.Items.Add( value );
						if( GameEngineApp.ShadowDirectionalLightTextureSize == value )
							comboBox.SelectedIndex = index;
					}
					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						GameEngineApp.ShadowDirectionalLightTextureSize = (int)sender.SelectedItem;
					};
				}

				////ShadowDirectionalLightMaxTextureCount
				//{
				//   comboBox = (EComboBox)pageVideo.Controls[ "ShadowDirectionalLightMaxTextureCount" ];
				//   for( int n = 0; n < 3; n++ )
				//   {
				//      int count = n + 1;
				//      comboBox.Items.Add( count );
				//      if( count == GameEngineApp.ShadowDirectionalLightMaxTextureCount )
				//         comboBox.SelectedIndex = n;
				//   }
				//   comboBox.SelectedIndexChange += delegate( EComboBox sender )
				//   {
				//      GameEngineApp.ShadowDirectionalLightMaxTextureCount = (int)sender.SelectedItem;
				//   };
				//}

				//ShadowSpotLightTextureSize
				{
					comboBox = (ComboBox)pageShadows.Controls[ "ShadowSpotLightTextureSize" ];
					for( int value = 256, index = 0; value <= 8192; value *= 2, index++ )
					{
						comboBox.Items.Add( value );
						if( GameEngineApp.ShadowSpotLightTextureSize == value )
							comboBox.SelectedIndex = index;
					}
					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						GameEngineApp.ShadowSpotLightTextureSize = (int)sender.SelectedItem;
					};
				}

				//ShadowSpotLightMaxTextureCount
				{
					comboBox = (ComboBox)pageShadows.Controls[ "ShadowSpotLightMaxTextureCount" ];
					for( int n = 0; n < 3; n++ )
					{
						int count = n + 1;
						comboBox.Items.Add( count );
						if( count == GameEngineApp.ShadowSpotLightMaxTextureCount )
							comboBox.SelectedIndex = n;
					}
					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						GameEngineApp.ShadowSpotLightMaxTextureCount = (int)sender.SelectedItem;
					};
				}

				//ShadowPointLightTextureSize
				{
					comboBox = (ComboBox)pageShadows.Controls[ "ShadowPointLightTextureSize" ];
					for( int value = 256, index = 0; value <= 8192; value *= 2, index++ )
					{
						comboBox.Items.Add( value );
						if( GameEngineApp.ShadowPointLightTextureSize == value )
							comboBox.SelectedIndex = index;
					}
					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						GameEngineApp.ShadowPointLightTextureSize = (int)sender.SelectedItem;
					};
				}

				//ShadowPointLightMaxTextureCount
				{
					comboBox = (ComboBox)pageShadows.Controls[ "ShadowPointLightMaxTextureCount" ];
					for( int n = 0; n < 3; n++ )
					{
						int count = n + 1;
						comboBox.Items.Add( count );
						if( count == GameEngineApp.ShadowPointLightMaxTextureCount )
							comboBox.SelectedIndex = n;
					}
					comboBox.SelectedIndexChange += delegate( ComboBox sender )
					{
						GameEngineApp.ShadowPointLightMaxTextureCount = (int)sender.SelectedItem;
					};
				}
			}

			//pageSound
			{
				bool enabled = SoundWorld.Instance.DriverName != "NULL";

				Control pageSound = tabControl.Controls[ "Sound" ];

				//soundVolumeCheckBox
				scrollBar = (ScrollBar)pageSound.Controls[ "SoundVolume" ];
				scrollBar.Value = enabled ? GameEngineApp.SoundVolume : 0;
				scrollBar.Enable = enabled;
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					GameEngineApp.SoundVolume = sender.Value;
				};

				//musicVolumeCheckBox
				scrollBar = (ScrollBar)pageSound.Controls[ "MusicVolume" ];
				scrollBar.Value = enabled ? GameEngineApp.MusicVolume : 0;
				scrollBar.Enable = enabled;
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					GameEngineApp.MusicVolume = sender.Value;
				};
			}


			#region pageControls
			//pageControls
			{
				Control pageControls = tabControl.Controls[ "Controls" ];
				
				cmbBoxDevice = (ComboBox)pageControls.Controls[ "InputDevices" ];
				axisfilterbutton = ((Button)pageControls.Controls["ChangeAxisfilter"]);
				btnAddBinding = ((Button)pageControls.Controls["btnAddBinding"]);
				btnAddBinding.Click += delegate(Button sender)
				{
					//if (btnAddBinding == null)
					//    return;
					CreateAdd_Custom_Control_Dialogue();
				};

				controlsList = pageControls.Controls[ "ListControls" ] as ListBox;
				
				//MouseHSensitivity
				scrollBar = (ScrollBar)pageControls.Controls[ "MouseHSensitivity" ];
				scrollBar.Value = GameControlsManager.Instance.MouseSensitivity.X;
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					Vec2 value = GameControlsManager.Instance.MouseSensitivity;
					value.X = sender.Value;
					GameControlsManager.Instance.MouseSensitivity = value;
				};

				//MouseVSensitivity
				scrollBar = (ScrollBar)pageControls.Controls[ "MouseVSensitivity" ];
				scrollBar.Value = Math.Abs( GameControlsManager.Instance.MouseSensitivity.Y );
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					Vec2 value = GameControlsManager.Instance.MouseSensitivity;
					bool invert = ( (CheckBox)pageControls.Controls[ "MouseVInvert" ] ).Checked;
					value.Y = sender.Value * ( invert ? -1.0f : 1.0f );
					GameControlsManager.Instance.MouseSensitivity = value;
				};

				//MouseVInvert
				checkBox = (CheckBox)pageControls.Controls[ "MouseVInvert" ];
				checkBox.Checked = GameControlsManager.Instance.MouseSensitivity.Y < 0;
				checkBox.CheckedChange += delegate( CheckBox sender )
				{
					Vec2 value = GameControlsManager.Instance.MouseSensitivity;
					value.Y =
						( (ScrollBar)pageControls.Controls[ "MouseVSensitivity" ] ).Value *
						( sender.Checked ? -1.0f : 1.0f );
					GameControlsManager.Instance.MouseSensitivity = value;
				};

				//AlwaysRun
				checkBox = (CheckBox)pageControls.Controls[ "AlwaysRun" ];
				checkBox.Checked = GameControlsManager.Instance.AlwaysRun;
				checkBox.CheckedChange += delegate( CheckBox sender )
				{
					GameControlsManager.Instance.AlwaysRun = sender.Checked;
				};


				//Devices
				comboBoxInputDevices = cmbBoxDevice;
				cmbBoxDevice.Items.Add( "Keyboard/Mouse" );
				if( InputDeviceManager.Instance != null )
				{
					foreach( InputDevice device in InputDeviceManager.Instance.Devices )
						cmbBoxDevice.Items.Add( device );
				}
				cmbBoxDevice.SelectedIndex = 0;
				UpdateBindedInputControlsListBox();

				cmbBoxDevice.SelectedIndexChange += delegate( ComboBox sender )
				{
					if( axisfilterbutton != null )
						axisfilterbutton.Enable = false;
					
					if( controlsList.SelectedIndex != -1 )
						controlsList.SelectedIndex = 0;
					
					UpdateBindedInputControlsListBox();
				};

				scrollBar = (ScrollBar)pageControls.Controls[ "DeadzoneVScroll" ];
				scrollBar.Value = GameControlsManager.Instance.DeadZone;
				textBox = (TextBox)pageControls.Controls[ "DeadZoneValue" ];
				textBox.Text = GameControlsManager.Instance.DeadZone.ToString();
				scrollBar.ValueChange += delegate( ScrollBar sender )
				{
					GameControlsManager.Instance.DeadZone = sender.Value;
					textBox.Text = sender.Value.ToString();
				};

				( (Button)pageControls.Controls[ "ControlSave" ] ).Click += delegate( Button sender )
				{
					GameControlsManager.Instance.SaveCustomConfig();
				};

				Control message = window.Controls[ "TabControl/Controls/ListControls/Message" ];
				
				controlsList.ItemMouseDoubleClick += delegate( object sender, ListBox.ItemMouseEventArgs e )
				{
					message.Text = "Type the new key (ESC to cancel)";
					message.ColorMultiplier = new ColorValue( 1, 0, 0 );
					Controls.Add( new KeyListener( sender ) );
				};

				controlsList.SelectedIndexChange += delegate( ListBox sender )
				{
					if( controlsList.SelectedItem == null || axisfilterbutton == null || !( controlsList.SelectedItem is GameControlsManager.SystemJoystickValue ) )
						return;

					var item = controlsList.SelectedItem as GameControlsManager.SystemJoystickValue;

					axisfilterbutton.Enable = item.Type == GameControlsManager.SystemJoystickValue.Types.Axis || item.Type == GameControlsManager.SystemJoystickValue.Types.Slider;

				};

				( (Button)pageControls.Controls[ "Default" ] ).Click += delegate( Button sender )
				{
					GameControlsManager.Instance.ResetKeyMouseSettings();
					GameControlsManager.Instance.ResetJoystickSettings();
					UpdateBindedInputControlsListBox();
				};

				//Incin -- change Axis Filter alone
				axisfilterbutton.Click += delegate(Button sender)
				{
					if (controlsList.SelectedItem == null || axisfilterbutton == null || !(controlsList.SelectedItem is GameControlsManager.SystemJoystickValue))
						return;

					var item = controlsList.SelectedItem as GameControlsManager.SystemJoystickValue;

					if (item.Type == GameControlsManager.SystemJoystickValue.Types.Axis || item.Type == GameControlsManager.SystemJoystickValue.Types.Slider)
						CreateAxisFilterDialogue();

				};
				axisfilterbutton.Enable = false;

				{
					if (controlsList.SelectedItem == null || axisfilterbutton == null || !(controlsList.SelectedItem is GameControlsManager.SystemJoystickValue))
						return;

					var item = controlsList.SelectedItem as GameControlsManager.SystemJoystickValue;

					if (item.Type == GameControlsManager.SystemJoystickValue.Types.Axis || item.Type == GameControlsManager.SystemJoystickValue.Types.Slider)
						CreateAxisFilterDialogue();

				};


				//Controls
				UpdateBindedInputControlsListBox();

				if( controlsList.SelectedIndex != -1 )
					controlsList.SelectedIndex = 0;
			}


			#endregion

			//pageLanguage
			{
				Control pageLanguage = tabControl.Controls[ "Language" ];

				//Language
				{
					comboBox = (ComboBox)pageLanguage.Controls[ "Language" ];

					List<string> languages = new List<string>();
					{
						languages.Add( "Autodetect" );
						string[] directories = VirtualDirectory.GetDirectories( LanguageManager.LanguagesDirectory, "*.*",
							SearchOption.TopDirectoryOnly );
						foreach( string directory in directories )
							languages.Add( Path.GetFileNameWithoutExtension( directory ) );
					}

					string language = "Autodetect";
					if( engineConfigBlock != null )
					{
						TextBlock localizationBlock = engineConfigBlock.FindChild( "Localization" );
						if( localizationBlock != null && localizationBlock.IsAttributeExist( "language" ) )
							language = localizationBlock.GetAttribute( "language" );
					}

					foreach( string lang in languages )
					{
						string displayName = lang;
						if( lang == "Autodetect" )
							displayName = Translate( lang );

						comboBox.Items.Add( new ComboBoxItem( lang, displayName ) );
						if( string.Compare( language, lang, true ) == 0 )
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
					}
					if( comboBox.SelectedIndex == -1 )
						comboBox.SelectedIndex = 0;

					comboBox.SelectedIndexChange += comboBoxLanguage_SelectedIndexChange;
				}

				//LanguageRestart
				{
					Button button = (Button)pageLanguage.Controls[ "LanguageRestart" ];
					button.Click += buttonLanguageRestart_Click;
				}
			}

			tabControl.SelectedIndex = lastPageIndex;
			tabControl.SelectedIndexChange += tabControl_SelectedIndexChange;
			UpdatePageButtonsState();
		}

		void UpdateShadowControlsEnable()
		{
			Control pageVideo = window.Controls[ "TabControl" ].Controls[ "Shadows" ];

			bool textureShadows =
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapLow ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapMedium ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapHigh ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapLowPSSM ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapMediumPSSM ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapHighPSSM;

			bool pssm = GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapLowPSSM ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapMediumPSSM ||
				GameEngineApp.ShadowTechnique == ShadowTechniques.ShadowmapHighPSSM;

			bool allowShadowColor = GameEngineApp.ShadowTechnique != ShadowTechniques.None;

			pageVideo.Controls[ "ShadowColor" ].Enable =
				!GameEngineApp.ShadowUseMapSettings && allowShadowColor;

			pageVideo.Controls[ "ShadowPSSMSplitFactor1" ].Enable =
				!GameEngineApp.ShadowUseMapSettings && pssm;

			pageVideo.Controls[ "ShadowPSSMSplitFactor2" ].Enable =
				!GameEngineApp.ShadowUseMapSettings && pssm;

			pageVideo.Controls[ "ShadowFarDistance" ].Enable =
				!GameEngineApp.ShadowUseMapSettings &&
				GameEngineApp.ShadowTechnique != ShadowTechniques.None;

			pageVideo.Controls[ "ShadowDirectionalLightTextureSize" ].Enable = textureShadows;
			//pageVideo.Controls[ "ShadowDirectionalLightMaxTextureCount" ].Enable = textureShadows;
			pageVideo.Controls[ "ShadowSpotLightTextureSize" ].Enable = textureShadows;
			pageVideo.Controls[ "ShadowSpotLightMaxTextureCount" ].Enable = textureShadows;
			pageVideo.Controls[ "ShadowPointLightTextureSize" ].Enable = textureShadows;
			pageVideo.Controls[ "ShadowPointLightMaxTextureCount" ].Enable = textureShadows;
		}

		void ChangeVideoMode()
		{
			Vec2I size;
			{
				size = EngineApp.Instance.VideoMode;

				if( comboBoxResolution.SelectedIndex != -1 )
				{
					string s = (string)( comboBoxResolution ).SelectedItem;
					s = s.Replace( "x", " " );
					size = Vec2I.Parse( s );
				}
			}

			EngineApp.Instance.VideoMode = size;
		}

		void UpdateBindedInputControlsListBox()
		{
			Control pageControls = window.Controls[ "TabControl" ].Controls[ "Controls" ];
			controlsList = pageControls.Controls[ "ListControls" ] as ListBox;

			controlsList.Items.Clear();

			var device = Devices.Keyboard;
			if( comboBoxInputDevices.SelectedIndex != 0 )
			{
				if( comboBoxInputDevices.SelectedItem.ToString().ToLower().Contains( "xbox360" ) )
				{
					device = Devices.Joystick_Xbox360;
				}
				else
				{
					device = Devices.Joystick;
				}
			}




			foreach( GameControlsManager.GameControlItem item in GameControlsManager.Instance.Items )
			{

				if( device == Devices.Keyboard )
				{
					if( item.BindedKeyboardMouseValues.Count > 0 )
					{
						foreach( var key in item.BindedKeyboardMouseValues )
						{
							controlsList.Items.Add( key );
						}

					}
					else
					{
						controlsList.Items.Add( new GameControlsManager.SystemKeyboardMouseValue() { Parent = item, Unbound = true } );
					}
				}
				else
				{
					var unbound = true;
					foreach( var key in item.bindedJoystickValues )
					{
						if (device == Devices.Joystick_Xbox360)
						{
							if( key.Type == GameControlsManager.SystemJoystickValue.Types.Button )
							{
								if( key.Button.ToString().ToLower().Contains( "xbox360" ) )
								{
									controlsList.Items.Add( key );
									unbound = false;
								}
							}
							else if( key.Type == GameControlsManager.SystemJoystickValue.Types.Axis )
							{
								if( key.Axis.ToString().ToLower().Contains( "xbox360" ) )
								{
									controlsList.Items.Add( key );
									unbound = false;
								}
							}
						}
						else
						{
							if( key.Type == GameControlsManager.SystemJoystickValue.Types.Button )
							{
								if( key.Button.ToString().ToLower().Contains( "xbox360" ) )
								{
									continue;
								}
							}
							else if( key.Type == GameControlsManager.SystemJoystickValue.Types.Axis )
							{
								if( key.Axis.ToString().ToLower().Contains( "xbox360" ) )
								{
									continue;
								}
							}
							controlsList.Items.Add( key );
							unbound = false;
						}

					}
					if( unbound )
					{
						controlsList.Items.Add( new GameControlsManager.SystemJoystickValue() { Parent = item, Unbound = true } );
					}

				}


			}


		}

		void CreateAxisFilterDialogue()
		{
			ComboBox comboBox;
			Control AxisFilterControl = ControlDeclarationManager.Instance.CreateControl( @"GUI\AxisFilter.gui" );
			AxisFilterControl.MouseCover = true;
			Controls.Add( AxisFilterControl );

			comboBox = (ComboBox)AxisFilterControl.Controls[ "cmbAxisFilter" ];
			//foreach( var value in Enum.GetValues( typeof( JoystickAxisFilters ) ) )
			//{
			//    comboBox.Items.Add( value );
			//}
			comboBox.Items.Add( JoystickAxisFilters.GreaterZero );
			comboBox.Items.Add( JoystickAxisFilters.LessZero );
			comboBox.Items.Add( JoystickAxisFilters.OnlyGreaterZero );
			comboBox.Items.Add( JoystickAxisFilters.OnlyLessZero );

			int index = (int)( controlsList.SelectedItem as GameControlsManager.SystemJoystickValue ).AxisFilter;
			index = index == 4 ? 0 : index;//hack to get the good index 
			comboBox.SelectedIndex = index;

			( (Button)AxisFilterControl.Controls[ "OK" ] ).Click += delegate( Button sender )
			{
				AxisFilterControl.SetShouldDetach();
				axisfilterselection = (JoystickAxisFilters)comboBox.SelectedItem;
				( controlsList.SelectedItem as GameControlsManager.SystemJoystickValue ).AxisFilter = axisfilterselection;
				controlsList.ItemButtons[ controlsList.SelectedIndex ].Text = controlsList.SelectedItem.ToString();
				axisfilterselection = JoystickAxisFilters.DEADZONE; //set back to Deadzone
			};

			( (Button)AxisFilterControl.Controls[ "Cancel" ] ).Click += delegate( Button sender )
			{
				AxisFilterControl.SetShouldDetach();
			};
		}

        /// <summary>
        /// CreateAdd_Custom_Control_Dialogue() support code
        /// </summary>

		TabControl MainOptionsTabControl;
        TabControl tabJoystickControlOptions;
		Button[] pageControlsButtons = new Button[3];
        Button[] pagejoystickButtons = new Button[3];
		static string message = null;
		static int lastPageIndex2;
        static int lastPageIndex3;
        
        void UpdatetabJoystickControlOptionsPageButtonsState()
        {
            for (int n = 0; n < pageControlsButtons.Length; n++)
            {
                Button button = pagejoystickButtons[n];
                button.Active = tabJoystickControlOptions.SelectedIndex == n;
            }
        }

        void tabJoystickControlOptions_SelectedIndexChange(TabControl sender)
        {
            lastPageIndex3 = sender.SelectedIndex;
            UpdatetabJoystickControlOptionsPageButtonsState();
        }

        void joystickPageTabButtons_Click(Button sender)
        {
            int index = Array.IndexOf(pagejoystickButtons, sender);
            tabJoystickControlOptions.SelectedIndex = index;
        }

		void UpdateMainOptionsPageButtonsState()
		{
			for (int n = 0; n < pageControlsButtons.Length; n++)
			{
				Button button = pageControlsButtons[n];
				button.Active = MainOptionsTabControl.SelectedIndex == n;
			}
		}

		void MainOptionsTabControl_SelectedIndexChange(TabControl sender)
		{
			lastPageIndex2 = sender.SelectedIndex;
			UpdateMainOptionsPageButtonsState();
		} 
       
        void pageControlsButton_Click(Button sender)
		{
			int index = Array.IndexOf(pageControlsButtons, sender);
			MainOptionsTabControl.SelectedIndex = index;
		}
		
		///<summary>
		/// ___ InCin filtering by combo boxes and listboxes --- all me
		///void CreateAdd_Custom_Control_Dialogue()
		///populate all drop downs and comboboxes
		///filter by device or device Type
		///hide unneeded info or unhide pages
		/// </summary>		
		void CreateAdd_Custom_Control_Dialogue()
		{
            //Enums for selected dropdowns to save back and compare 
            Devices devicetype_selected;// = (Devices)cmbDeviceType.SelectedItem;
            GameControlKeys control_selected;// = (Control)lstCommand.SelectedItem;
            // group Keyboard
            EKeys lstKeyboardButtonChoices_selected;// = (EKeys)lstKeyboardButtonChoices.SelectedItem;
			// group Mouse
            EMouseButtons cmbMouseButtonChoices_selected; // = (EMouseButtons)cmbMouseButtonChoices.SelectedItem;
            MouseScroll cmbMouseScrollChoices_selected; //= (MouseScroll)cmbMouseScrollChoices.SelectedItem;
            //group joystick //slider options
            JoystickSliders cmbSliderChoices_selected; //= (JoystickSliders)cmbSliderChoices.SelectedItem;
            JoystickSliderAxes cmbSliderAxisChoices_selected; //= (JoystickSliderAxes)cmbSliderAxisChoices.SelectedItem;
            JoystickAxisFilters cmbSliderAxisFilterChoices_selected; // = (JoystickAxisFilters)cmbSliderAxisFilterChoices.SelectedItem;
            //axis filter
            JoystickSliderAxes cmbAxisChoices_selected; // = (JoystickSliderAxes)cmbAxisChoices.SelectedItem;
            JoystickAxisFilters cmbAxisFilterChoices_selected;// = (JoystickAxisFilters)cmbAxisFilterChoices.SelectedItem;
            //buttons
            JoystickButtons lstJoyButtonChoices_selected;// = (JoystickButtons)lstJoyButtonChoices.SelectedItem;
            float Strength_selected = 1.0f; //always max strength

			Control Add_Custom_Control = ControlDeclarationManager.Instance.CreateControl(@"GUI\Add_Custom_Control.gui");
			Add_Custom_Control.MouseCover = true;
			Controls.Add(Add_Custom_Control);

			#region AddCustomControl.Gui

			#region MainControls

			ComboBox cmbDeviceType;
			cmbDeviceType = (ComboBox)Add_Custom_Control.Controls["cmbDeviceType"];
			cmbDeviceType.Items.Add("< Nothing Selected >");
			foreach (var value in Enum.GetValues(typeof(Devices)))
			{
				if(!(value.ToString().Contains(Devices.GetServices.ToString())) && !(value.ToString().Contains(Devices.All_Devices.ToString()))) //exclude for internal use

					cmbDeviceType.Items.Add(value);
				
			}
			cmbDeviceType.SelectedIndex = 0;
			ComboBox cmbDevice;
			cmbDevice = (ComboBox)Add_Custom_Control.Controls["cmbDevice"];
			cmbDevice.Items.Add("< Nothing Selected >");
			cmbDevice.Items.Add("Keyboard"); //unhandled object as device
			cmbDevice.Items.Add("Mouse");   //unhandled object as a device
			if (InputDeviceManager.Instance != null)
			{
				foreach (InputDevice devicename in InputDeviceManager.Instance.Devices)
					cmbDevice.Items.Add(devicename); //handled objects
				//filter 
			}

			Control cntrlCommands = (Control)Add_Custom_Control.Controls["cntrlCommands"];
			cntrlCommands.Visible = false;
			//Commands Available
			ListBox lstCommand;
			lstCommand = (ListBox)Add_Custom_Control.Controls["cntrlCommands"].Controls["lstCommand"];
			lstCommand.Items.Add("< Nothing Selected >");
			lstCommand.SelectedIndex = 0;
			foreach (var value in Enum.GetValues(typeof(GameControlKeys)))
			{
				lstCommand.Items.Add(value);
			}


			//control Tab Controls
			//TabControl MainOptionsTabControl;
			MainOptionsTabControl = (TabControl)Add_Custom_Control.Controls["MainOptionsTabControl"];
			MainOptionsTabControl.SelectedIndexChange += MainOptionsTabControl_SelectedIndexChange;
			MainOptionsTabControl.Visible = false;

			ScrollBar scrlSelectedStrength;
			scrlSelectedStrength = (ScrollBar)MainOptionsTabControl.Controls["scrlSelectedStrength"];

            scrlSelectedStrength.ValueChange += delegate(ScrollBar sender)
            {
                Strength_selected = sender.Value;
            };

			pageControlsButtons[0] = (Button)Add_Custom_Control.Controls["MainOptionsTabControl"].Controls["btnMouseOptions"];
			pageControlsButtons[1] = (Button)Add_Custom_Control.Controls["MainOptionsTabControl"].Controls["btnKeyboardOptions"];
			pageControlsButtons[2] = (Button)Add_Custom_Control.Controls["MainOptionsTabControl"].Controls["btnJoystickOptions"];
			foreach (Button pageButton in pageControlsButtons)
			{
				pageButton.Click += new Button.ClickDelegate( pageControlsButton_Click );
			}

			TextBox lblMessage = (TextBox)Add_Custom_Control.Controls["lblMessage"];

			#endregion MainControls
			#region pageMouseoptions
			
			Control pageMouseOptions;
			pageMouseOptions = (Control)Add_Custom_Control.Controls["MainOptionsTabControl"].Controls["pageMouseOptions"];
			//Page visible= false || true;

			#region MouseTabControls

			//MainOptionsTabControl.MouseTabControl.pageMouseButtonOptions
			TabControl MouseTabControl = (TabControl)pageMouseOptions.Controls["MouseTabControl"];

			ComboBox cmbMouseButtonChoices;
			cmbMouseButtonChoices = (ComboBox)MouseTabControl.Controls["pageMouseButtonOptions"].Controls["cmbMouseButtonChoices"];
			cmbMouseButtonChoices.Items.Add("< Nothing Selected >");
			foreach (var value in Enum.GetValues(typeof(EMouseButtons)))
			{
				cmbMouseButtonChoices.Items.Add(value);
			}
			cmbMouseButtonChoices.SelectedIndex = 0;

			//MainOptionsTabControl.MouseTabControl.pageMouseScrollOptions.cmbMouseScrollChoices
			ComboBox cmbMouseScrollChoices;
			cmbMouseScrollChoices = (ComboBox)MouseTabControl.Controls["pageMouseScrollOptions"].Controls["cmbMouseScrollChoices"];
			cmbMouseScrollChoices.Items.Add("< Nothing Selected >");
			foreach (var value in Enum.GetValues(typeof(MouseScroll)))
			{
				cmbMouseScrollChoices.Items.Add(value);
			}
			cmbMouseScrollChoices.SelectedIndex = 0;
			#endregion
			#endregion pageMouseOptions
			#region pageKeyboardOptions

			Control pageKeyboardOptions = (Control)Add_Custom_Control.Controls["MainOptionsTabControl"].Controls["pageKeyboardOptions"];
			//visible false true?

			ListBox lstKeyboardButtonChoices;
			lstKeyboardButtonChoices = (ListBox)pageKeyboardOptions.Controls["lstKeyboardButtonChoices"];
			lstKeyboardButtonChoices.Items.Add("< Nothing Selected >");
			foreach (var value in Enum.GetValues(typeof(EKeys)))
			{
				lstKeyboardButtonChoices.Items.Add(value);
			}
			lstKeyboardButtonChoices.SelectedIndex = 0;
			//if (lstKeyboardButtonChoices.ItemButtons["lstKeyboardButtonChoices"]Text.Contains("<Nothing Selected>") != null)
			//    lstKeyboardButtonChoices.SelectedIndex = -1;
			#endregion pageKeyboardOptions

			//MainOptionsTabControl.pageJoystickOptions
			//tabJoystickControlOptions
			#region pageJoystickOptions
				Control pageJoystickOptions;
				pageJoystickOptions = (Control)Add_Custom_Control.Controls["MainOptionsTabControl"].Controls["pageJoystickOptions"];//.Controls["tabJoystickControlOptions"];
				tabJoystickControlOptions = (TabControl)pageJoystickOptions.Controls["tabJoystickControlOptions"];

                pagejoystickButtons[0] = (Button)tabJoystickControlOptions.Controls["btnSliderOptions"];
                pagejoystickButtons[1] = (Button)tabJoystickControlOptions.Controls["btnAxisOptions"];
                pagejoystickButtons[2] = (Button)tabJoystickControlOptions.Controls["btnButtonOptions"];
                foreach (Button pageButton in pagejoystickButtons)
                {
                    pageButton.Click += new Button.ClickDelegate(joystickPageTabButtons_Click);
                }

				#region pageSliderOptions
					Control pageSliderOptions = tabJoystickControlOptions.Controls["pageSliderOptions"];
					#region cmbSliderChoices
						ComboBox cmbSliderChoices;
						
						cmbSliderChoices = (ComboBox)pageSliderOptions.Controls["cmbSliderChoices"];
						cmbSliderChoices.Items.Add("< Nothing Selected >");
						foreach (var value in Enum.GetValues(typeof(JoystickSliders)))
						{
							cmbSliderChoices.Items.Add(value);
						}
						cmbSliderChoices.SelectedIndex = 0;

						ComboBox cmbSliderAxisChoices;
						cmbSliderAxisChoices = (ComboBox)pageSliderOptions.Controls["cmbSliderAxisChoices"];
                        cmbSliderAxisChoices.Items.Add("< Nothing Selected >");
						foreach (var value in Enum.GetValues(typeof(JoystickSliderAxes)))
						{
							cmbSliderAxisChoices.Items.Add(value);
						}
						cmbSliderAxisChoices.SelectedIndex = 0;

						ComboBox cmbSliderAxisFilterChoices;
						cmbSliderAxisFilterChoices = (ComboBox)pageSliderOptions.Controls["cmbSliderAxisFilterChoices"];
						cmbSliderAxisFilterChoices.Items.Add("< Nothing Selected >");
						foreach (var value in Enum.GetValues(typeof(JoystickAxisFilters)))
						{
							cmbSliderAxisFilterChoices.Items.Add(value);
						}
						cmbSliderAxisFilterChoices.SelectedIndex = 0;
					#endregion cmbSliderChoices
				#endregion pageSliderOptions       
				#region pageAxisOptions
					Control pageAxisOptions = tabJoystickControlOptions.Controls["pageAxisOptions"];
					ComboBox cmbAxisChoices;
					cmbAxisChoices = (ComboBox)pageAxisOptions.Controls["cmbAxisChoices"];
					cmbAxisChoices.Items.Add("< Nothing Selected >");
					foreach (var value in Enum.GetValues(typeof(JoystickSliderAxes)))
					{
						cmbAxisChoices.Items.Add(value);
					}
					cmbAxisChoices.SelectedIndex = 0;


					ComboBox cmbAxisFilterChoices;
					cmbAxisFilterChoices = (ComboBox)pageAxisOptions.Controls["cmbAxisFilterChoices"];
					cmbAxisFilterChoices.Items.Add("< Nothing Selected >");
					foreach (var value in Enum.GetValues(typeof(JoystickAxisFilters )))
					{
						cmbAxisFilterChoices.Items.Add(value);
					}
					cmbAxisFilterChoices.SelectedIndex = 0;

					Control pageJoystickButtonOptions = tabJoystickControlOptions.Controls["pageJoystickButtonOptions"];
					ListBox lstJoyButtonChoices = (ListBox)pageJoystickButtonOptions.Controls["lstJoyButtonChoices"];
					lstJoyButtonChoices.Items.Add("<Nothing Selected>");
					foreach (var value in Enum.GetValues(typeof(JoystickButtons)))
					{
						lstJoyButtonChoices.Items.Add(value);
					}
					lstJoyButtonChoices.SelectedIndex = 0;
				#endregion pageAxisOptions
			#endregion pageJoystickOptions

			//___ InCin filtering by combo boxes and listboxes --- all me
			// setting all indexes at 0 -- tracks "< Nothing Selected >"
			#region IndexChanged
			//if(page visible == enabled){

			cmbMouseButtonChoices.SelectedIndexChange += delegate(ComboBox sender)
			{
				message = null;
				if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0) //< Nothing Selected >
					return;

				if (sender.SelectedIndex == 0)
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message +=" MouseButton: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
				}
				else
				{	
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " MouseButton: " + sender.SelectedItem.ToString();
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
				}
				lblMessage.Text = message;
			};

			cmbMouseScrollChoices.SelectedIndexChange += delegate(ComboBox sender)
			{
				message = null;
				if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0)// < Nothing Selected >
					return;

				if (sender.SelectedIndex == 0)
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " MouseScroll: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
                    lstKeyboardButtonChoices.Visible = false;
				}
				else
				{	
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " MouseScroll: " + sender.SelectedItem.ToString();
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
                    lstKeyboardButtonChoices.Visible = true;
				}
				lblMessage.Text = message;
			};

			lstKeyboardButtonChoices.SelectedIndexChange += delegate(ListBox sender)
			{
				message = null;
                MainOptionsTabControl.Visible = false;
				if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0)
					return;

				if (sender.SelectedIndex == 0) // < Nothing Selected >
				{
					//< Nothing Selected >
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " KeyboardButton: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString(); 
				}
				else
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " KeyboardButton: " + sender.SelectedItem.ToString();
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
				}
				lblMessage.Text = message;	
			};

			//}
			//if(page visible == enabled)
			cmbSliderChoices.SelectedIndexChange += delegate(ComboBox sender)
			{
				message = null;
		        MainOptionsTabControl.Visible = false;
                if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0)  // < Nothing Selected >
				{
					return;
				}

				if (sender.SelectedIndex == 0)
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " Slider: < Nothing Selected >";
					message += " Axis: < Nothing Selected >";
					message += " AxisFilter: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
					cmbSliderChoices.Visible = false;

				}
				else
				{	
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " Slider: " + sender.SelectedItem.ToString(); //sender
					message += " Axis: < Nothing Selected >";
					message += " AxisFilter: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
                    cmbSliderAxisChoices.Visible = true;
				}
				lblMessage.Text = message;	
			};

			cmbSliderAxisChoices.SelectedIndexChange += delegate(ComboBox sender)
			{
				message = null;
				if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0  || cmbSliderChoices.SelectedIndex == 0) // < Nothing Selected >
					return;

				if (sender.SelectedIndex == 0)
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " Slider: " + cmbSliderChoices.SelectedItem.ToString();
					message += " Axis: < Nothing Selected >";
					message += " AxisFilter: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
                    cmbSliderAxisFilterChoices.Visible = false;
				}
				else
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message +=" Command: " + lstCommand.SelectedItem.ToString();
					message += " Slider: " + cmbSliderChoices.SelectedItem.ToString();
					message += " Axis: " + sender.SelectedItem.ToString();
					message += " AxisFilter: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
                    cmbSliderAxisFilterChoices.Visible = true;
				}
				lblMessage.Text = message;
			};

			cmbSliderAxisFilterChoices.SelectedIndexChange += delegate(ComboBox sender)
			{
				message = null;
				if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0 || cmbSliderChoices.SelectedIndex == 0 || cmbSliderAxisChoices.SelectedIndex == 0)// < Nothing Selected >
					return;

				if (sender.SelectedIndex == 0)
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " Slider: " + cmbSliderChoices.SelectedItem.ToString();
					message += " Axis: " + cmbSliderAxisChoices.SelectedItem.ToString();
					message += " AxisFilter: < Nothing Selected >";
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
				}
				else 
				{
					message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
					message += " Command: " + lstCommand.SelectedItem.ToString();
					message += " Slider: " + cmbSliderChoices.SelectedItem;
					message += " Axis: " + cmbSliderChoices.SelectedItem.ToString();
					message += " AxisFilter: " + sender.SelectedItem.ToString();
					message += " Strength: " + scrlSelectedStrength.Value.ToString();
				}
				lblMessage.Text = message;
			};
			//}
			//if(page visible == enabled){
            cmbAxisChoices.SelectedIndexChange += delegate(ComboBox sender)
            {
                message = null;
                if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0)// < Nothing Selected >
                {
                    cntrlCommands.Visible = false;
                    MainOptionsTabControl.Visible = false;
                    return;
                }

                if (cmbAxisChoices.SelectedIndex == 0)
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " Axis: < Nothing Selected >";
                    message += " AxisFilter: < Nothing Selected >";
                    message += " Strength: " + scrlSelectedStrength.Value.ToString();
                }
                else
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " Axis: " + sender.SelectedItem.ToString();
                    message += " AxisFilter: < Nothing Selected >";
                    message += " Strength: " + scrlSelectedStrength.Value.ToString();

                }
                lblMessage.Text = message;
                
            };

            cmbAxisFilterChoices.SelectedIndexChange += delegate(ComboBox sender)
            {
                message = null;
                if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0 || cmbAxisChoices.SelectedIndex == 0)
                {

                    MainOptionsTabControl.Visible = false;
                    return;
                }
                if (cmbAxisFilterChoices.SelectedIndex == 0)
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " Axis: " + cmbAxisChoices.SelectedItem.ToString();
                    message += " AxisFilter: < Nothing Selected >";
                    message += " Strength: " + scrlSelectedStrength.Value.ToString();
                }
                else
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " Axis: " + cmbAxisChoices.SelectedItem.ToString();
                    message += " AxisFilter: " + sender.SelectedItem.ToString();
                    message += " Strength: " + scrlSelectedStrength.Value.ToString();

                }
                lblMessage.Text = message;
            };

            lstJoyButtonChoices.SelectedIndexChange += delegate(ListBox sender)
            {
                message = null;
                if (cmbDeviceType.SelectedIndex == 0 || lstCommand.SelectedIndex == 0)
                {
                    //MainOptionsTabControl.Visible = false;

                    return;
                }
                if (lstJoyButtonChoices.SelectedIndex == 0)
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " JoystickButton: < Nothing Selected >";
                    message += " Strength: " + scrlSelectedStrength.Value.ToString();

                }
                else
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " JoystickButton: " + sender.SelectedItem.ToString();
                    message += " Strength: " + scrlSelectedStrength.Value.ToString();
                }
                lblMessage.Text = message;
            };
            //}


            lstCommand.SelectedIndexChange += delegate(ListBox sender)
            {
                message = null;
                if (cmbDeviceType.SelectedIndex == 0)
                    return;

                if (sender.SelectedIndex == 0)
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + "< Nothing Selected >";
                    message += " Bind: < Nothing Selected >";
                    message += "Strength: " + scrlSelectedStrength.Value.ToString();
                    MainOptionsTabControl.Visible = false;
                }
                else
                {
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: " + lstCommand.SelectedItem.ToString();
                    message += " Bind: < Nothing Selected >";
                    message += "Strength: " + scrlSelectedStrength.Value.ToString();
                    MainOptionsTabControl.Visible = true;
                }
                lblMessage.Text = message;
            };

            #region comboDeviceType
            ///<summary>
            /// Filters down through each TabControl hides what isn't needed
            /// MainOptionsTabControl
            /// MouseTabControl
            ///</summary> 
            //Primary choices
            cmbDeviceType.SelectedIndexChange += delegate(ComboBox sender)
            {
                message = null;
                if (sender.SelectedIndex == -1)
                    return;
                if (sender.SelectedIndex != 0)
                {
                    Devices devicetype = (Devices)sender.SelectedItem;
                    cntrlCommands.Visible = true;
                    switch (devicetype)
                    {
                        case Devices.Mouse:
                            {
                                pageControlsButton_Click(pageControlsButtons[0]);
                                pageControlsButtons[0].Enable = true;
                                pageControlsButtons[1].Enable = false;
                                pageControlsButtons[2].Enable = false;
                                break;
                            }
                        case Devices.Keyboard:
                            {
                                pageControlsButton_Click(pageControlsButtons[1]);
                                pageControlsButtons[0].Enable = false;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = false;
                                break;
                            }

                        case Devices.Joystick:
                            {
                                pageControlsButton_Click(pageControlsButtons[2]);
                                pageControlsButtons[0].Enable = false;
                                pageControlsButtons[1].Enable = false;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }

                        case Devices.Joystick_Xbox360:
                            {
                                pageControlsButton_Click(pageControlsButtons[2]);
                                pageControlsButtons[0].Enable = false;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }
                        case Devices.Joystick_Playstation:
                            {
                                pageControlsButton_Click(pageControlsButtons[2]);
                                pageControlsButtons[0].Enable = false;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }
                        case Devices.Joystick_WII:
                            {
                                pageControlsButton_Click(pageControlsButtons[2]);
                                pageControlsButtons[0].Enable = false;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }
                        case Devices.Custom_Audio:
                            {
                                pageControlsButton_Click(pageControlsButtons[2]);
                                pageControlsButtons[0].Enable = true;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }
                        case Devices.Custom:
                            {
                                pageControlsButton_Click(pageControlsButtons[0]);
                                pageControlsButtons[0].Enable = true;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }
                        default:
                            {
                                pageControlsButton_Click(pageControlsButtons[0]);
                                pageControlsButtons[0].Enable = true;
                                pageControlsButtons[1].Enable = true;
                                pageControlsButtons[2].Enable = true;
                                break;
                            }
                    }
                    message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                    message += " Command: <Nothing Selected >";
                    message += " Bind: < Nothing Selected >";
                    message += "Strength: " + scrlSelectedStrength.Value.ToString();

                    if (lstCommand.SelectedIndex != 0)
                    {
                        message = "DeviceType: " + cmbDeviceType.SelectedItem.ToString();
                        message += " Command: " + lstCommand.SelectedItem.ToString();
                        message += " Bind: < Nothing Selected >";
                        message += "Strength: " + scrlSelectedStrength.Value.ToString();
                    }
                }
                else
                {
                    message = " < Nothing selected >";
                    pageControlsButtons[0].Enable = true;
                    pageControlsButtons[1].Enable = true;
                    pageControlsButtons[2].Enable = true;
                }
                lblMessage.Text = message;
            };
            #endregion comboDeviceType

            #region comboDevice

            //cmbDevice.SelectedIndexChange += delegate(ComboBox sender)
            //{
            //    //if (sender.SelectedIndex != -1)
            //    //{
            //    //    InputDevice inputdevices = (InputDevice)sender.SelectedItem;
            //    //    //Set deviceType
            //    //    // continue
            //    //}
            //    //lblMessage.Text = message;
            //    ;
            //    //hidden for now
            //};
            #endregion comboDevice

            #endregion IndexChanged

            #region ButtonOK

            ((Button)Add_Custom_Control.Controls["buttonOK"]).Click += delegate(Button sender)
            {
                devicetype_selected = (Devices)cmbDeviceType.SelectedItem;
                control_selected = (GameControlKeys)lstCommand.SelectedItem;

                if (devicetype_selected == null || control_selected == null)
                {
                    return;
                }

                //if (pageControlsButtons[0].Enable == true || pageControlsButtons[1].Enable == true || pageControlsButtons[2].Enable == true)

                switch (devicetype_selected) //devicetype
                {
                    case Devices.Keyboard:
                        {
                            // group Keyboard
                            lstKeyboardButtonChoices_selected = (EKeys)lstKeyboardButtonChoices.SelectedItem;
                            if (lstKeyboardButtonChoices_selected == null)
                                return;
                            break;
                        }
                    case Devices.Mouse:
                        {
                            // group Mouse
                            cmbMouseButtonChoices_selected = (EMouseButtons)cmbMouseButtonChoices.SelectedItem;
                            if(cmbMouseButtonChoices_selected == null)
                                return;
                            cmbMouseScrollChoices_selected = (MouseScroll)cmbMouseScrollChoices.SelectedItem;
                            if(cmbMouseScrollChoices_selected == null)
                                return;
                            break;
                        }

                    case Devices.Joystick:
                    case Devices.Joystick_Playstation:
                    case Devices.Joystick_WII:
                    case Devices.Joystick_Xbox360:
                        {
                            //Filter by tab
                            //group joystick //slider options
                            cmbSliderChoices_selected = (JoystickSliders)cmbSliderChoices.SelectedItem;
                            if (cmbSliderChoices_selected == null)
                                return;
                            cmbSliderAxisChoices_selected = (JoystickSliderAxes)cmbSliderAxisChoices.SelectedItem;
                            if (cmbSliderAxisChoices_selected == null)
                                return;
                            cmbSliderAxisFilterChoices_selected = (JoystickAxisFilters)cmbSliderAxisFilterChoices.SelectedItem;
                            if (cmbSliderAxisFilterChoices_selected == null)
                                return;
                            
                            //axis filter
                            cmbAxisChoices_selected  = (JoystickSliderAxes)cmbAxisChoices.SelectedItem;
                            if (cmbAxisChoices_selected == null)
                                return;
                            cmbAxisFilterChoices_selected = (JoystickAxisFilters)cmbAxisFilterChoices.SelectedItem;
                            if (cmbAxisFilterChoices_selected == null)
                                return;
                            //buttons
                            lstJoyButtonChoices_selected = (JoystickButtons)lstJoyButtonChoices.SelectedItem;
                            if (lstJoyButtonChoices_selected == null)
                                return;
                            break;
                        }

                    case Devices.Custom:
                        {
                            break;
                        }
                    case Devices.Custom_Audio:
                        {
                            break;
                        }
                }
                //save bind
                Add_Custom_Control.SetShouldDetach(); 
            };
            #endregion ButtonOK

            ((Button)Add_Custom_Control.Controls["buttonReset"]).Click += delegate(Button sender)
            {
                cmbDevice.SelectedIndex = 0;
                cmbDeviceType.SelectedIndex = 0;
                lstCommand.SelectedIndex = 0; //should be lstCommand
                cmbMouseButtonChoices.SelectedIndex = 0;
                cmbMouseScrollChoices.SelectedIndex = 0;
                lstKeyboardButtonChoices.SelectedIndex = 0; //< Nothing Selected >
                cmbSliderChoices.SelectedIndex = 0;
                cmbSliderAxisChoices.SelectedIndex = 0;
                cmbSliderAxisFilterChoices.SelectedIndex = 0;
                cmbAxisChoices.SelectedIndex = 0;
                cmbAxisFilterChoices.SelectedIndex = 0;
                lstJoyButtonChoices.SelectedIndex = 0; //<Nothing Selected>
                cntrlCommands.Visible = false;
                MainOptionsTabControl.Visible = false;

                message = " < Nothing selected >";
                lblMessage.Text = message;
            };

            ((Button)Add_Custom_Control.Controls["buttonCancel"]).Click += delegate(Button sender)
            {
                Add_Custom_Control.SetShouldDetach();
            };

        #endregion

            MainOptionsTabControl.SelectedIndex = lastPageIndex2;
            UpdateMainOptionsPageButtonsState();
        }

		protected override void OnControlDetach( Control control )
		{
			if( control as KeyListener != null && window.Controls.Count > 0 )
			{
				Control message = window.Controls[ "TabControl/Controls/ListControls/Message" ];
				message.Text = " Double click to change the key";
				message.ColorMultiplier = new ColorValue( 1, 1, 1 );
				UpdateBindedInputControlsListBox();
			}

			base.OnControlDetach( control );
		}

		protected override bool OnMouseWheel( int delta )
		{
			if( base.OnMouseWheel( delta ) )
				return true;
			return false;
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( base.OnKeyDown( e ) )
				return true;
			if( e.Key == EKeys.Escape )
			{
				SetShouldDetach();
				return true;
			}
			return false;
		}

		protected override void OnDetach()
		{
			GameControlsManager.Instance.SaveCustomConfig();
			base.OnDetach();
		}

		void tabControl_SelectedIndexChange( TabControl sender )
		{
			lastPageIndex = sender.SelectedIndex;
			UpdatePageButtonsState();
		}



		TextBlock LoadEngineConfig()
		{
			string fileName = VirtualFileSystem.GetRealPathByVirtual( "user:Configs/Engine.config" );
			string error;
			return TextBlockUtils.LoadFromRealFile( fileName, out error );
		}

		void SaveEngineConfig( TextBlock engineConfigBlock )
		{
			string fileName = VirtualFileSystem.GetRealPathByVirtual( "user:Configs/Engine.config" );
			try
			{
				string directoryName = Path.GetDirectoryName( fileName );
				if( directoryName != "" && !Directory.Exists( directoryName ) )
					Directory.CreateDirectory( directoryName );
				using( StreamWriter writer = new StreamWriter( fileName ) )
				{
					writer.Write( engineConfigBlock.DumpToString() );
				}
			}
			catch( Exception e )
			{
				Log.Warning( "Unable to save file \"{0}\". {1}", fileName, e.Message );
			}
		}

		void comboBoxRenderTechnique_SelectedIndexChange( ComboBox sender )
		{
			//update Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			if( engineConfigBlock == null )
				engineConfigBlock = new TextBlock();
			TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
			if( rendererBlock == null )
				rendererBlock = engineConfigBlock.AddChild( "Renderer" );
			ComboBoxItem item = (ComboBoxItem)sender.SelectedItem;
			rendererBlock.SetAttribute( "renderTechnique", item.Identifier );
			SaveEngineConfig( engineConfigBlock );

			EnableVideoRestartButton();
		}

		void comboBoxFiltering_SelectedIndexChange( ComboBox sender )
		{
			//update Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			if( engineConfigBlock == null )
				engineConfigBlock = new TextBlock();
			TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
			if( rendererBlock == null )
				rendererBlock = engineConfigBlock.AddChild( "Renderer" );
			ComboBoxItem item = (ComboBoxItem)sender.SelectedItem;
			rendererBlock.SetAttribute( "filtering", item.Identifier );
			SaveEngineConfig( engineConfigBlock );

			EnableVideoRestartButton();
		}

		void checkBoxDepthBufferAccess_CheckedChange( CheckBox sender )
		{
			//update Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			if( engineConfigBlock == null )
				engineConfigBlock = new TextBlock();
			TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
			if( rendererBlock == null )
				rendererBlock = engineConfigBlock.AddChild( "Renderer" );
			rendererBlock.SetAttribute( "depthBufferAccess", sender.Checked.ToString() );
			SaveEngineConfig( engineConfigBlock );

			EnableVideoRestartButton();

			UpdateComboBoxAntialiasing();
		}

		void comboBoxAntialiasing_SelectedIndexChange( ComboBox sender )
		{
			//update Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			if( engineConfigBlock == null )
				engineConfigBlock = new TextBlock();
			TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
			if( rendererBlock == null )
				rendererBlock = engineConfigBlock.AddChild( "Renderer" );
			if( comboBoxAntialiasing.SelectedIndex != -1 )
			{
				ComboBoxItem item = (ComboBoxItem)comboBoxAntialiasing.SelectedItem;
				rendererBlock.SetAttribute( "fullSceneAntialiasing", item.Identifier );
			}
			else
				rendererBlock.DeleteAttribute( "fullSceneAntialiasing" );
			SaveEngineConfig( engineConfigBlock );

			EnableVideoRestartButton();
		}

		void UpdateComboBoxAntialiasing()
		{
			int lastSelectedIndex = comboBoxAntialiasing.SelectedIndex;

			comboBoxAntialiasing.Items.Clear();

			comboBoxAntialiasing.Items.Add( new ComboBoxItem( "RecommendedSetting", Translate( "Recommended setting" ) ) );
			comboBoxAntialiasing.Items.Add( new ComboBoxItem( "0", Translate( "No" ) ) );
			if( !checkBoxDepthBufferAccess.Checked )
			{
				comboBoxAntialiasing.Items.Add( new ComboBoxItem( "2", "2" ) );
				comboBoxAntialiasing.Items.Add( new ComboBoxItem( "4", "4" ) );
				comboBoxAntialiasing.Items.Add( new ComboBoxItem( "6", "6" ) );
				comboBoxAntialiasing.Items.Add( new ComboBoxItem( "8", "8" ) );
			}
			comboBoxAntialiasing.Items.Add( new ComboBoxItem( "FXAA", Translate( "Fast Approximate AA (FXAA)" ) ) );

			if( lastSelectedIndex >= 0 && lastSelectedIndex <= 1 )
				comboBoxAntialiasing.SelectedIndex = lastSelectedIndex;
			else
				comboBoxAntialiasing.SelectedIndex = 0;
		}

		void checkBoxVerticalSync_CheckedChange( CheckBox sender )
		{
			//update Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			if( engineConfigBlock == null )
				engineConfigBlock = new TextBlock();
			TextBlock rendererBlock = engineConfigBlock.FindChild( "Renderer" );
			if( rendererBlock == null )
				rendererBlock = engineConfigBlock.AddChild( "Renderer" );
			rendererBlock.SetAttribute( "verticalSync", sender.Checked.ToString() );
			SaveEngineConfig( engineConfigBlock );

			EnableVideoRestartButton();
		}

		void EnableVideoRestartButton()
		{
			Control pageVideo = window.Controls[ "TabControl" ].Controls[ "Video" ];
			Button button = (Button)pageVideo.Controls[ "VideoRestart" ];
			button.Enable = true;
		}

		void buttonVideoRestart_Click( Button sender )
		{
			Program.needRestartApplication = true;
			EngineApp.Instance.SetNeedExit();
		}

		void comboBoxLanguage_SelectedIndexChange( ComboBox sender )
		{
			//update Engine.config
			TextBlock engineConfigBlock = LoadEngineConfig();
			if( engineConfigBlock == null )
				engineConfigBlock = new TextBlock();
			TextBlock localizationBlock = engineConfigBlock.FindChild( "Localization" );
			if( localizationBlock == null )
				localizationBlock = engineConfigBlock.AddChild( "Localization" );
			ComboBoxItem item = (ComboBoxItem)sender.SelectedItem;
			localizationBlock.SetAttribute( "language", item.Identifier );
			SaveEngineConfig( engineConfigBlock );

			EnableLanguageRestartButton();
		}

		void EnableLanguageRestartButton()
		{
			Control pageLanguage = window.Controls[ "TabControl" ].Controls[ "Language" ];
			Button button = (Button)pageLanguage.Controls[ "LanguageRestart" ];
			button.Enable = true;
		}

		void buttonLanguageRestart_Click( Button sender )
		{
			Program.needRestartApplication = true;
			EngineApp.Instance.SetNeedExit();
		}

		string Translate( string text )
		{
			return LanguageManager.Instance.Translate( "UISystem", text );
		}

		void pageButton_Click( Button sender )
		{
			int index = Array.IndexOf( pageButtons, sender );
			tabControl.SelectedIndex = index;
		}

		void UpdatePageButtonsState()
		{
			for( int n = 0; n < pageButtons.Length; n++ )
			{
				Button button = pageButtons[ n ];
				button.Active = tabControl.SelectedIndex == n;
			}
		}
	}
}
