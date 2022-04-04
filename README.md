# Crestron Home Driver - Lutron HomeWorks QS System

> Crestron Home Driver -to control switching(LQSE-4S1-D) and motor(LQSE-4M-D) devices

> Support custom icons (Please check the [Icon Library](https://sdkcon78221.crestron.com/sdk/Crestron_Certified_Drivers_SDK/Content/Extension-Device-Icons.pdf) for the supported detail icon list)

## Building the driver

***For each visual studio project, remember to import the NuGet Crestron Library and the required SDK following the instruction [here](<https://sdkcon78221.crestron.com/sdk/Crestron_Certified_Drivers_SDK/Content/Topics/Create-a-Project.htm>).***

1. Open Visual Studio 2019 and open the LutronSwitchingDevice (*LutronSwitchingDevice.sln*) project and build the project solution
2. Open the LutronMotorDevice (*LutronMotorDevice.sln*) project and build the project solution.
3. Open the LutronHWQSGateway (*LutronHWQSGateway.sln*) project.
4. Add the reference to the *LutronMotorDevice.dll* and *LutronSwitchingDevice.dll*
5. Build the project solution. Run the ManifestUtil.exe which will generate three pkg files (*LutronHWQSGateway.pkg*, *LutronMotorDevice.pkg*, *LutronSwitchingDevice.pkg*)
6. Move the *LutronHWQSGateway.pkg* to the ThridPartyDriver/Import Directory of your Crestron Home Controller using Crestron Toolbox.

## Project Structure

    ├──LutronHWQSGateway                   # Implementation of the Gateway Driver
        ├── LutronHWQSGateway
            ├── ALutronHWQSGateway.cs       # Entry point of the driver
            ├── ALutronHWQSGateway.json     # Driver Definition File
            ├── LutronHWQSGatewayProtocol.cs # TCP Protocol class for TCP communication
            ├── LutronRXParser.cs            # Parse the incoming RX data
            ├── LutronCommand.cs            # Helper class to create CommandSet object
    ├── LutronMotorDevice
        ├── LutronMotorDevice
            ├── ALutronMotorDevice.cs       # Entry point of the Home Extension Driver for motor devices
    ├── LutronSwitchingDevice
        ├── LutronSwitchingDevice
            ├── ALutronSwitchingDevice.cs   # Entry point of the Home Extension Driver for switching devices
    ├── IncludeInPkg                        # All the UIDefinition and translation files
