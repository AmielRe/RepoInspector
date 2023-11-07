# RepoInspector

A simple command line application that will detect and notify suspicious behavior in an integrated
GitHub organization.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Installation](#installation)
  - [Configuration](#configuration)
- [Usage](#usage)
- [Authors](#authors)

## Features

Explain the key features and functionality of your console application.

- Currently 3 anomalies active
- Easy to configure existing anomalies
- Easy to create new anomalies and configure them

## Prerequisites

Before running the application, go over the following prerequisites

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/3.1)

## Getting Started

Follow the following steps to run the application locally

### Installation

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/AmielRe/RepoInspector.git
   ```

2. Change directory to the project folder:

   ```bash
   cd RepoInspector/RepoInspector
   ```

### Configuration

Use the default configuration found in appsettings.json file or change them as you like

In order to run the application locally, change the property "SmeeURL" with your [Smee.io Proxy URL](https://smee.io/) proxy url:

```json
{
 ...
 "SmeeUrl": "<your_smee_url>",
 ...
}
```

In addition, add environment variable locally with the secret used for the webhook:

```bash
setx GITHUB_SECRET "<your_secret_string>"
```

## Usage

Run the application using the following command in command-line:

```bash
dotnet run
```

Add webhook in your Github organization:

- In the "Payload URL" write the Smee.io Proxy URL (Same from the configuration file).
- In the "Secret" write your secret string (must be matched with the "GITHUB_SECRET" environment variable set before).

Currently, the app supports 3 anomalies for:

- Pushing code between 14:00-16:00
- Creating a team with the prefix “hacker”
- Creating a repository and deleting it in less than 10 minutes

## Authors

- **Amiel Refaeli** - [Github](https://github.com/AmielRe)
