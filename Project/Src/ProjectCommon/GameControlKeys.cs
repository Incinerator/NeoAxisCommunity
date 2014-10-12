// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.EntitySystem;

namespace ProjectCommon
{
	public enum GameControlKeys
	{
		///////////////////////////////////////////
		//Moving

		[DefaultKeyboardMouseValue( EKeys.W )]
		[DefaultKeyboardMouseValue( EKeys.Up )]
		[DefaultJoystickValue( JoystickAxes.Y, JoystickAxisFilters.GreaterZero, 1 )]
		[DefaultJoystickValue( JoystickAxes.XBox360_LeftThumbstickY, JoystickAxisFilters.GreaterZero, 1)]
		[DefaultJoystickValue( JoystickSliders.Slider1, JoystickSliderAxes.X, JoystickAxisFilters.OnlyGreaterZero , 1)]
		Forward,

		[DefaultKeyboardMouseValue( EKeys.S )]
		[DefaultKeyboardMouseValue( EKeys.Down )]
		[DefaultJoystickValue( JoystickAxes.Y, JoystickAxisFilters.LessZero, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_LeftThumbstickY, JoystickAxisFilters.LessZero, 1)]
		[DefaultJoystickValue( JoystickSliders.Slider1, JoystickSliderAxes.X, JoystickAxisFilters.OnlyLessZero, 1)]
		Backward,

		[DefaultKeyboardMouseValue( EKeys.A )]
		[DefaultKeyboardMouseValue( EKeys.Left )]
		[DefaultJoystickValue( JoystickAxes.X, JoystickAxisFilters.LessZero, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_LeftThumbstickX, JoystickAxisFilters.LessZero, 1)]
		Left,

		[DefaultKeyboardMouseValue( EKeys.D )]
		[DefaultKeyboardMouseValue( EKeys.Right )]
		[DefaultJoystickValue( JoystickAxes.X, JoystickAxisFilters.GreaterZero, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_LeftThumbstickX, JoystickAxisFilters.GreaterZero, 1)]
		Right,

		///////////////////////////////////////////
		//Looking

		[DefaultJoystickValue( JoystickAxes.Rz, JoystickAxisFilters.GreaterZero, 1 )]
		[DefaultJoystickValue( JoystickAxes.XBox360_RightThumbstickY, JoystickAxisFilters.GreaterZero, 1)]
		//MouseMove (in the PlayerIntellect)
		LookUp,

		[DefaultJoystickValue( JoystickAxes.Rz, JoystickAxisFilters.LessZero, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_RightThumbstickY, JoystickAxisFilters.LessZero, 1)]
		//MouseMove (in the PlayerIntellect)
		LookDown,

		[DefaultJoystickValue( JoystickAxes.Z, JoystickAxisFilters.LessZero, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_RightThumbstickX, JoystickAxisFilters.LessZero , 1)]
		//MouseMove (in the PlayerIntellect)
		LookLeft,

		[DefaultJoystickValue( JoystickAxes.Z, JoystickAxisFilters.GreaterZero, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_RightThumbstickX, JoystickAxisFilters.GreaterZero, 1 )]
		//MouseMove (in the PlayerIntellect)
		LookRight,

		///////////////////////////////////////////
		//Actions

		[DefaultKeyboardMouseValue( EMouseButtons.Left)]
		[DefaultJoystickValue( JoystickButtons.Button1, 1)]
		[DefaultJoystickValue( JoystickAxes.XBox360_RightTrigger, JoystickAxisFilters.GreaterZero ,1)]
		Fire1,

		[DefaultKeyboardMouseValue( EMouseButtons.Right )]
		[DefaultJoystickValue( JoystickButtons.Button2, 1 )]
		[DefaultJoystickValue( JoystickAxes.XBox360_LeftTrigger, JoystickAxisFilters.GreaterZero, 1 )]
		Fire2,

		[DefaultKeyboardMouseValue( EKeys.Space )]
		[DefaultJoystickValue( JoystickButtons.Button3, 1)]
		[DefaultJoystickValue( JoystickButtons.XBox360_A, 1)]
		Jump,

		[DefaultKeyboardMouseValue( EKeys.C )]
        [DefaultJoystickValue(JoystickButtons.Button6, 1)]
        [DefaultJoystickValue(JoystickButtons.XBox360_B, 1)]
		Crouching,

		[DefaultKeyboardMouseValue( EKeys.R )]
        [DefaultJoystickValue(JoystickButtons.Button4, 1)]
        [DefaultJoystickValue(JoystickButtons.XBox360_LeftShoulder, 1)]
		Reload,

		[DefaultKeyboardMouseValue( EKeys.E )]
        [DefaultJoystickValue(JoystickButtons.Button5, 1)]
        [DefaultJoystickValue(JoystickButtons.XBox360_RightShoulder, 1)]
		Use,

		[DefaultJoystickValue( JoystickPOVs.POV1, JoystickPOVDirections.West, 1)]
		PreviousWeapon,

		[DefaultJoystickValue( JoystickPOVs.POV1, JoystickPOVDirections.East , 1)]
		NextWeapon,

		[DefaultKeyboardMouseValue( EKeys.D1 )]
		Weapon1,
		[DefaultKeyboardMouseValue( EKeys.D2 )]
		Weapon2,
		[DefaultKeyboardMouseValue( EKeys.D3 )]
		Weapon3,
		[DefaultKeyboardMouseValue( EKeys.D4 )]
		Weapon4,
		[DefaultKeyboardMouseValue( EKeys.D5 )]
		Weapon5,
		[DefaultKeyboardMouseValue( EKeys.D6 )]
		Weapon6,
		[DefaultKeyboardMouseValue( EKeys.D7 )]
		Weapon7,
		[DefaultKeyboardMouseValue( EKeys.D8 )]
		Weapon8,
		[DefaultKeyboardMouseValue( EKeys.D9 )]
		Weapon9,

		[DefaultKeyboardMouseValue( EKeys.Shift )]
		Run,

		//Vehicle
		[DefaultKeyboardMouseValue( EKeys.Z )]
		[DefaultJoystickValue( JoystickPOVs.POV1, JoystickPOVDirections.North , 1)]
		VehicleGearUp,
		[DefaultKeyboardMouseValue( EKeys.X )]
		[DefaultJoystickValue( JoystickPOVs.POV1, JoystickPOVDirections.South , 1)]
		VehicleGearDown,
		[DefaultKeyboardMouseValue( EKeys.Space )]
		VehicleHandbrake,
	}
}
