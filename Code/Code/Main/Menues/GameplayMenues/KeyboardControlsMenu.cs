﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsymptoticMonoGameFramework {

    public class KeyboardControlsMenu : ScrollingMenuScreen {
        
        private MenuButton backButton;
        private MenuButton resetToDefaultButton;

        private bool waitingForAllKeysToBeUnpressed = false; //must unpress all keys after clicking a button so the control isn't immediately assigned to that button
        private bool waitingForKeyPress = false;
        private int buttonIndexPressed = -1;

        public KeyboardControlsMenu() {
            Setup();
        }

        public KeyboardControlsMenu(MenuScreen _parentMenu) : base(_parentMenu) {
            Setup();
        }

        private void Setup() {
            buttonSize = MenuButton.buttonSize + new Vector2(0, 12);
            buttonPadding = 4;
        }

        public override void MenuScreenOpened() {
            base.MenuScreenOpened();
            UpdateAllButtonImages();
        }

        public override void LoadContent() {
            base.LoadContent();

            SpriteFont arial22Font = Globals.content.Load<SpriteFont>("Fonts/arial-bold-22");

            foreach (KeyValuePair<string, object> entry in DefaultControls.keyboardControls) {
                AddButton(new KeyboardControlsMenuButton(
                        new Vector2(),
                        this,
                        buttonSize,
                        entry.Key,
                        arial22Font,
                        ControlsConfig.keyboardControls[entry.Key][0]
                    ));
            }

            resetToDefaultButton = new MenuButton(
                    new Vector2(),
                    this,
                    buttonSize,
                    "Reset to Default",
                    arial22Font
                );
            AddButton(resetToDefaultButton);

            backButton = new MenuButton(
                    new Vector2(),
                    this,
                    buttonSize,
                    "Back",
                    arial22Font
                );
            AddButton(backButton);
        }

        public override void UnloadContent() {
            base.UnloadContent();
        }

        protected override void BackPressed() {
            ControlsConfig.SaveControlsSettings();
            currentlySelectedButtonIndex = 0;
            parentMenu.CloseSubMenu();
        }

        public override void Update(GameTime gameTime) {
            if (!waitingForKeyPress) {
                base.Update(gameTime);
            } else {
                if (!waitingForAllKeysToBeUnpressed) {
                    if (KeyboardInputPressed()) {
                        ((KeyboardControlsMenuButton)buttonList[buttonIndexPressed]).SetNewInput(KeyPressed());
                        waitingForKeyPress = false;
                    }
                } else if (!KeyboardInputPressed()) {
                    waitingForAllKeysToBeUnpressed = false;
                }
            }
        }

        public override void ButtonClicked(MenuButton button) {
            base.ButtonClicked(button);
            if (button == backButton) {
                BackButtonPressed();
            } else if (button == resetToDefaultButton) {
                ResetToDefaultButtonPressed();
            } else {
                KeyboardControlsMenuButton controlsButton = (KeyboardControlsMenuButton)button;
                if (controlsButton.buttonType == KeyboardControlsMenuButtonType.Button) {
                    controlsButton.SetWaitingForInput();
                    buttonIndexPressed = buttonList.IndexOf(button);
                    waitingForAllKeysToBeUnpressed = true;
                    waitingForKeyPress = true;
                } else if (controlsButton.buttonType == KeyboardControlsMenuButtonType.Toggle) {
                    bool currentToggle = (ControlsConfig.keyboardControls[controlsButton.controlsTKey][0] == (int)ToggleOptions.True);
                    controlsButton.SetNewInput(!currentToggle);
                }
            }
        }
        
        private bool KeyboardInputPressed() {
            return Keyboard.GetState().GetPressedKeys().Length > 0 || Mouse.GetState().LeftButton == ButtonState.Pressed || 
                    Mouse.GetState().RightButton == ButtonState.Pressed || Mouse.GetState().MiddleButton == ButtonState.Pressed;
        }

        private int KeyPressed() {
            if (Keyboard.GetState().GetPressedKeys().Length > 0) {
                Keys[] keysPressed = Keyboard.GetState().GetPressedKeys();
                return (int)keysPressed[0];
            } else if(Mouse.GetState().LeftButton == ButtonState.Pressed){
                return (int)MouseClickOptions.LeftClick;
            } else if (Mouse.GetState().RightButton == ButtonState.Pressed) {
                return (int)MouseClickOptions.RightClick;
            } else if (Mouse.GetState().MiddleButton == ButtonState.Pressed) {
                return (int)MouseClickOptions.MiddleClick;
            }
            return 0;
        }

        private void BackButtonPressed() {
            BackPressed();
        }

        private void ResetToDefaultButtonPressed() {
            ControlsConfig.ResetToDefault(ControlsConfig.keyboardControllerIndex);
            UpdateAllButtonImages();
        }

        public void UpdateAllButtonImages() {
            foreach (MenuButton button in buttonList) {
                if (button is KeyboardControlsMenuButton) {
                    ((KeyboardControlsMenuButton)button).UpdateImage();
                }
            }
        }

        public bool CurrentlySettingControl() {
            return waitingForKeyPress;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
        }
    }
}
