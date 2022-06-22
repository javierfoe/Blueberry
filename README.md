# Blueberry
Asset that allows Multiplayer games to be played through Bluetooth and using Mirror between Android devices.

First clone the repository, download the unity packages from the following releases of Mirror and Ignorance, and unpack them.

Mirror:

https://github.com/vis2k/Mirror/releases/tag/v67.1.0

Ignorance:

https://github.com/SoftwareGuy/Ignorance/releases/tag/v1.4.0r2

This project uses this abandoned asset on the Unity Asset Store

Android Bluetooth Multiplayer:
https://assetstore.unity.com/packages/tools/network/android-bluetooth-multiplayer-basic-20928

## How to use
The most basic setup is to replace `KcpTransport` for `IgnoranceTransport.Ignorance`, `NetworkManagerHUD` for `BlueberryHUD` and add `BlueberryHelper` component on `NetworkManager` gameObject.

In case `HUD` components are not used, `BlueberryHelper` methods `StartHost`,`StartClient` and `StartServer` need to be called instead of those from `NetworkManager` as done in the `Chat` scene.

With this setup you can make builds for both `Windows Standalone` and `Android` devices without changing the code. Depending on the target platform `BlueberryHelper` will trigger the `Bluetooth` functionality or just behave as a standard `NetworkManager`. This will hopefully ease the development time.
