# Load Shedder Game

This repository contains simple game for bilancing of the electric grid. 
It was designed by request of [Aqualectra](https://www.aqualectra.com) company for [Kaya Kaya festival](https://www.kayakaya.org/). 

![obrazek](https://github.com/fyziktom/LoadShedder/assets/78320021/d540abb5-a845-414f-82ef-9f9306cfcaec)

## Requirements

### Software

The code is for Visual Studio 2022. 

It uses [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) and component library [Blazorise](https://blazorise.com/). I recommend to search the documentation of this library for knowledge of the components. It is available [here](https://blazorise.com/docs). The demo containing all library components is [here](https://bootstrapdemo.blazorise.com/) and I recommend to check the [utilities documentation](https://blazorise.com/docs/helpers/utilities) to understand basic tools for Aligments, Flex and other css related settings which are wrapped by Blazorise (it allows to switch between css providers easilly without changing of the styling in code). 

The color theme can be override centrally like you can see in the VEFramework VEBlazor library. It need to define class with the color codes like [this](https://github.com/fyziktom/VirtualEconomyFramework/blob/main/VirtualEconomyFramework/VEBlazor/Models/Themes/DefaultTheme.cs) and then you can select the theme in the start of the [App.razor](https://github.com/fyziktom/VirtualEconomyFramework/blob/b45f80e7db34f403d60e6697ba6a5f352a86a8a1/VirtualEconomyFramework/VEBlazor.Demo.VENFTApp.Client/App.razor#L7).


The [VEDriversLite.EntitiesBlocks](https://github.com/fyziktom/VirtualEconomyFramework/wiki/EntitiesBlocks---Z%C3%A1kladn%C3%AD-%C3%BAvod-do-knihovny-VEDriversLite.EntitiesBlocks) library is used or energetic calculation. You can find more details in the [VEFramework Wiki](https://github.com/fyziktom/VirtualEconomyFramework/wiki#entitiesblocks).

Software can run on multiple platforms based on the possibilities of the .NET 7 (later will be switched to the .NET 8). 

There possibility to use just the virtual GameBoard and run the app in the cloud as you can see on the demo. 

It is not clear now how to switch between HW dependent and not HW dependent game same as multiplayer and signleplayer in the server backend. Here is the [component which simulate GameBoard](https://github.com/fyziktom/LoadShedder/blob/main/src/LoadShedder/LoadShedder/Components/AllGamePiecesKeyboard.razor). For the multiplayer as in online demo you need to also create new Player, Game and GameBoard instances for each game. With real HW you are keeping same ID of the GameBoard and you add new Player and Game. You can see this process in this [GameStart component](https://github.com/fyziktom/LoadShedder/blob/ff649fec85c2669d05072011dc4e0560bc51b9ca/src/LoadShedder/LoadShedder/Components/GameStartComponent.razor#L286C7-L286C7). If you do not add new GameBoard all players would submit the data to one GameBoard which is "testBoard" in default. Default configs are created here in [LoadShedderCoreService](https://github.com/fyziktom/LoadShedder/blob/ff649fec85c2669d05072011dc4e0560bc51b9ca/src/LoadShedder/LoadShedder/LoadShedderCoreService.cs#L154).

### Hardware
The game can run in both "only software" or with physical hardware. 

The game on the Kaya Kaya festival runned with combination of the physical board and data grabber with [M5Core basic module - based on ESP32](https://docs.m5stack.com/en/core/basic) with external two parallel ADC 16channel multiplexers [74HC4067PW-Q100J](https://cz.mouser.com/ProductDetail/Nexperia/74HC4067PW-Q100J?qs=sGAEpiMZZMutXGli8Ay4kMM331gquzwp6j58R3ncuS8%3D). 

I have chosen the Automotive version because of bigger reliability. It also contains ESD protections and limits the current to the ADC of the MCU (internal resistance of MUX is about 80 Ohms). The HW was supersimplified now and it is not the best version. It needs to decrease the voltage on the dividers to just 3.3V and maybe add additional protection diodes.

The schematic will be added later after improvements. It is practically M5Core plugged to the MUXs with resistor divider. The data inputs of the MUXs can be put in parallel so the MCU drives them both same time. The outputs of the MUXs goes to separated ADC channels so the MCU can read them "same time". 

The communication between the server and the MCU is through the Wi-Fi and HTTP API requests.

Photo of the hardware:

#### ADCs MUXs Board

![IMG_20230831_004228_619](https://github.com/fyziktom/LoadShedder/assets/78320021/76392112-45da-4365-9925-22aa4dcf85c0)

It contains two of MUXs in stack. I have used [this PCB convertor](https://cz.mouser.com/ProductDetail/910-PA0036). It is better to buy the ICs already soldered for example on [Sparkfun](https://www.sparkfun.com/products/9056). 


#### ADCs board plugged to M5Core Basic

![IMG_20230831_163314_907](https://github.com/fyziktom/LoadShedder/assets/78320021/3dc83621-16fa-4f93-806a-00e67093e5c5)

#### Signal measuring

One ADC input channel with plugged different value (linear line) to each ADC MUX input:

![IMG_20230825_215444_945](https://github.com/fyziktom/LoadShedder/assets/78320021/a4fa9b81-0efc-432d-ba14-825ca9fd7640)

Noise on the ADC input:

![IMG_20230825_181256_321](https://github.com/fyziktom/LoadShedder/assets/78320021/c137b729-5718-4524-9152-e48e128622f3)

Both ADC channels with some unpluged/different values:

![IMG_20230825_223736_327](https://github.com/fyziktom/LoadShedder/assets/78320021/fda8823f-8e11-4d77-95b6-e3fd55749482)


#### Detail of one of the GamePieces 
![IMG_20230828_155207_129](https://github.com/fyziktom/LoadShedder/assets/78320021/d7fdf30a-0302-4748-9ca9-95b67b062d78)


#### Full Game Board 

![IMG_20230902_141118_897](https://github.com/fyziktom/LoadShedder/assets/78320021/4c2456ac-10ec-4a50-a353-ba5c4bcfd9c2)


### Firmware

The FW is very simple. It is written in the C/C++ in Arduino studio. You can find the [firmware here](https://github.com/fyziktom/LoadShedder/blob/main/fw/LoadShedder/LoadShedder.ino).

For loading firmware to the M5Core module please check the documentation of the [M5Stack](https://docs.m5stack.com/en/quick_start/m5core/arduino).

The firmware needs to fill some wifi SSID and Pass to connect module and the server in the same network. You need to fill also server [IP into the module](https://github.com/fyziktom/LoadShedder/blob/ff649fec85c2669d05072011dc4e0560bc51b9ca/fw/LoadShedder/LoadShedder.ino#L56).

Firmware can do averaging on median calculation for each input value. The measurement goes multiple time and then averaged/median value is provided for software. [Here you can change sampling rate and amount of measurements for avg/med](https://github.com/fyziktom/LoadShedder/blob/ff649fec85c2669d05072011dc4e0560bc51b9ca/fw/LoadShedder/LoadShedder.ino#L37). Switch between the average and median must be done by uncomment/comment line [here](https://github.com/fyziktom/LoadShedder/blob/ff649fec85c2669d05072011dc4e0560bc51b9ca/fw/LoadShedder/LoadShedder.ino#L317) now. I will add it to defines later.

## Game principle

- Start with loading energy sources to the network until you will reach some suitable level (based on settings is 75MW now)
- Load the consumers up to the <10MW level.
- Bilance the network to get 0MW bilance.

## Contribution

Contribution is welcome. Easiest way is create fork of the repository. Then you can clone repository localy to your environment. The repository contains Developlment Branch which should be used to create branch from for the new issues.

If you have any request for feature or bug report please create issue for it. Then you can create branch for the specific issue. If you will solve the issue please create pull request so we can add the contribution to the main branch.

## Online demo

The online demo is available [here](https://loadshedder.azurewebsites.net/).

## Building app

If you have Visual Studio 2022 with the .NET 7 ASP.NET SDK you should be able to compile the project without any additional settings. 

### App settins
Most of the common settings you can do with [appsettings.json](https://github.com/fyziktom/LoadShedder/blob/main/src/LoadShedder/LoadShedder/appsettings.json) or for debug mode [appsettings.Development.json](https://github.com/fyziktom/LoadShedder/blob/main/src/LoadShedder/LoadShedder/appsettings.Development.json).

Please search the config file because most of the settings contstants are self explanatory.

### GamePieces settings
The game contains GamePieces. These are pshysical bricks wich has some unique identification (in our case it is unique value of the resistor inside of GamePiece). This unique value is matched in the software with some predefined name, description, expected measured voltage,  represented imaginary energy value in eGrid, etc. Those can be placed in the JSON config manually or you can load them with the registration utility in UI. 

The list of the AllowedGamePieces is defined for the each [GameBoard](https://github.com/fyziktom/LoadShedder/blob/main/src/LoadShedder/LoadShedder/Models/GameBoard.cs) and its [Positions](https://github.com/fyziktom/LoadShedder/blob/main/src/LoadShedder/LoadShedder/Models/Position.cs). 

Example of the configuration of one Position is here:

```json
      "1cfe7a44-515c-4beb-9044-c32ed8a9ea1c": {
        "Id": "1cfe7a44-515c-4beb-9044-c32ed8a9ea1c",
        "Name": "21-Jan_Thiel",
        "DeviceId": "test",
        "ChannelId": "27",
        "GameBoardId": "testBoard",
        "ChannelInputNumber": 27,
        "ActualPlacedGamePiece": null,
        "AllowedGamePieces": {
          "399": {
            "Id": "399",
            "Name": "21-Jan_Thiel - 15 MW",
            "Description": "",
            "EnergyValue": 15000.0,
            "IsPlugged": false,
            "ExpectedVoltage": 399.0,
            "ResistorsCombo": 0,
            "ResistorsCombo1": 0,
            "GamePieceType": 1
          },
          "913": {
            "Id": "913",
            "Name": "21-Jan_Thiel - 10 MW",
            "Description": "",
            "EnergyValue": 10000.0,
            "IsPlugged": false,
            "ExpectedVoltage": 913.0,
            "ResistorsCombo": 0,
            "ResistorsCombo1": 0,
            "GamePieceType": 1
          },
          "535": {
            "Id": "535",
            "Name": "21-Jan_Thiel - 5 MW",
            "Description": "",
            "EnergyValue": 5000.0,
            "IsPlugged": false,
            "ExpectedVoltage": 535.0,
            "ResistorsCombo": 0,
            "ResistorsCombo1": 0,
            "GamePieceType": 1
          }
        }
      },
```

If you run the app you can use the [Registration modal](https://github.com/fyziktom/LoadShedder/blob/ff649fec85c2669d05072011dc4e0560bc51b9ca/src/LoadShedder/LoadShedder/Components/DataDisplay.razor#L34) which is part of the [DataDisplay component](https://github.com/fyziktom/LoadShedder/blob/main/src/LoadShedder/LoadShedder/Components/DataDisplay.razor) now. It should be separated to own component soon.


