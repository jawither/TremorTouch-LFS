# TremorTouch

TremorTouch is a application that allows for users who experience tremors to make accurate and meaningful inputs on their touch screen devices. 

## Installation
### (Option 1:) Simulate the app inside Unity
If you'd like run the app yourself from the source code, it will require several steps. Keep in mind the project directory is quite large and may take some time to set up.

1. Go to the Unity download page (https://unity.com/download) and click on the "Download Unity Hub" button. Once the Unity Hub installer is downloaded, run it and follow the instructions to install it on your computer.

2. Once Unity Hub is installed, open it and click on the "Installs" tab. Click on the "Add" button and select "Unity 2021.3.17f1" from the list of available versions.

3. Once Unity installation is complete, clone this github repository. Because the repository directory is so large, you may have issues cloning directly, in which case you can download the .zip file from the github page.
	
4. From the Unity Hub, click Open and navigate the TremorTouch folder, and open If it tells you the folder you selected is invalid, you probably selected the TremorTouch-LFS folder. Make sure you're selecting the TremorTouch folder located inside the TremorTouch-LFS folder.

5. Once the project is open in the Unity app, go to the assets folder in the bottom left and open the scenes folder. Double click on the scene titled "SampleScene".

6. Near the top of the screen, click on the Tab titled "Game". Beneath that is another row of tabs. Click the other tab titled "Game" and select "Simulator". In the lower row of tabs, select "Samsung Galaxy S5 Neo from the list of phones. Any device should work, but our app was formatted and tested for this device.

7. In the upper right corner of the lower tab, select the drop down that says "Play Focus" and change it to "Play Maximized". You can now click the triangle play button at the top of the window to launch the app on a simulated device.

### (Option 2:) Run on mobile device with provided pre-built `.xcodeproj`
If you'd like to avoid the steps in Option 2, you can run our pre-made build on your iOS device. This requires you are using macOS and have XCode installed.

1. Download the `Build` sub-directory of the repository.

2. Make sure your iOS device is connected to your macOS device.

3. Inside is `Unity-iPhone.xcodeproj`. Open this in XCode and it will begin to index the files. This may take a couple minutes.

4. Click on the Unity-iPhone application in the upper-left corner of the XCode file explorer. Several tabs should appear (General, Signing & Capabilities...). Click on Signing & Capabilities, and under the All sub-tab, check 'Automatically Manage Signing'. Under Team, choose your personal account. In the 'Bundle Identifier' field, enter `tremorTouch`.

5. Click the triangle play button in the upper left and it should begin to build. If successful, an app should appear on your iOS device with the Unity logo as its icon. You can open this app to begin using TremorTouch.

## Usage

Using TremorTouch is as simple as tapping your screen as you normally would. Once the minimum tap threshold is met, a red dot will appear on the screen representing where the tap will be executed.

When ready to make the tap, simply stop making taps and after a set amount of time the tap will execute.

To press and hold, continue making taps until the dot turns green. A hold will be executed at that location until there is no input for a set amount of time.
