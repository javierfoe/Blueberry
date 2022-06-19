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
You can program your code as it were a standalone PC application, and once you are finished you just need to change a few things to make it Bluetooth compatible. In the next section you can see how it has been done for 3 different Mirror example scenes.

## Examples

### Basic and Multiple Matches Scenes
The only change needed is to remove `NetworkManagerHUD` and `KcpTransport` components from the `NetworkManager` gameObject and add `BlueberryNetworkManagerHelper`, `BlueberryHUD` and `Ignorance` transport.

### Chat Scene
This requires `KcpTransport` to be removed and `BlueberryNetworkManagerHelper` and `Ignorance` transport to be added.
`HostButton` and `ClientButton` must point to the `BlueberryNetworkManagerHelper` component and call `StartHost` and `StartClient` respectively.

