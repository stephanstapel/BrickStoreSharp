[![NuGet](https://img.shields.io/nuget/v/BrickStoreSharp?color=blue)](https://www.nuget.org/packages/BrickStoreSharp/)

# BrickStoreSharp
.net Adapter for reading and writing BrickStore inventory files

## Sponsoring
Implementing and maintaining this library is a lot of hard work. I'm doing this in my spare time, there is no company behind developing BrickStoreSharp. Support me in this work and help making this library better:

[:heart: Sponsor me on github](https://github.com/sponsors/stephanstapel)

## Overview
BrickStoreSharp is a .NET Standard library designed for handling BrickStore inventory files with ease and efficiency. This library allows users to read and write BrickStore inventory files (https://github.com/rgriebl/brickstore), providing a streamlined approach for managing and manipulating LEGO brick inventories.
Features

* Read and write BrickStore inventory files
* Compatible with .NET Standard 2.0
* Simple and intuitive API

## Installation

To install BrickStoreSharp, you can use the NuGet package manager:

```
Install-Package BrickStoreSharp
```
You can find more information about the nuget package here:

[![NuGet](https://img.shields.io/nuget/v/BrickStoreSharp?color=blue)](https://www.nuget.org/packages/BrickStoreSharp/)

Alternatively, you can clone this repository and build the library from source.

## Usage

Here's a quick example to get you started:

```csharp

using BrickStoreSharp;

// Load an inventory file
var inventory = BrickStoreInventory.Load("path/to/inventoryFile.bsx");

// Manipulate your inventory
// ...

// Save changes to a new file
inventory.Save("path/to/newInventoryFile.bsx");
```

For more detailed usage instructions, please refer to the documentation.

## Contact and contributing
We welcome contributions to BrickStoreSharp! If you have suggestions or want to contribute code, please feel free to open an issue or create a pull request.

## License
BrickStoreSharp is released under the Apache License, Version 2.0. See http://www.apache.org/licenses/LICENSE-2.0.html for details.
