# TremorTouch

TremorTouch is a application that allows for users who experience tremors to make accurate and meaningful inputs on their touch screen devices. 

## Installation

    Go to the Unity download page (https://unity.com/download) and click on the "Download Unity Hub" button.

    Once the Unity Hub installer is downloaded, run it and follow the instructions to install it on your computer.

    Once Unity Hub is installed, open it and click on the "Installs" tab.

    Click on the "Add" button and select "Unity 2021.3.17f1" from the list of available versions.

	Once installation is complete, clone this github repository either throught the command line.
	
	If cloning from the command line isn't working, or if you're running into issues with LFS, you can just download the zip file from the github page.
	
	From the Unity Hub, click Open and navigate the TremorTouch folder, and open
	
	If it tells you the folder you selected is invalid, you probably selected the TremorTouch-LFS folder.
	Maker sure you're selecting the TremorTouch folder located inside the TremorTouch-LFS folder.
	
	Once the project is open, go to the assets folder in the bottom left and open the scenes folder.
	
	Double click on the scene titled "SampleScene"
	
	Near the top of the screen, click on the Tab titled "Game"
	
	Beneath that is another row of tabs. Click the other tab titled "Game" and select "Simulator"
	
	In the lower row of tabs, select "Samsung Galaxy S5 Neo from the list of phones
	
	In the upper right corner of the lower tab, select the drop down that says "Play Focus" and change it to "Play Maximized"

## Usage

Using TremorTouch is as simple as tapping your screen as you normally would. Once the minimum tap threshold is met, a red dot will appear on the screen representing where the tap will be executed.

When ready to make the tap, simply stop making taps and after a set amount of time the tap will execute.

To press and hold, continue making taps until the dot turns green. A hold will be executed at that location until there is no input for a set amount of time.
