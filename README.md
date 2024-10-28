# Order Creator

## What is it?

This is a command line app that will create orders for a specified vertical for a given seller using valid product data, and randomized customer data.

## Configuration

### Set up your vertical list

Verticals (aka Marketplacer instances) are 1st configured in `appsettings.json`, an example of the vertical configuration is shown below:

```json
{
  "verticals": [
    {
      "id": "1",
      "name": "Best Friend Bazaar - Staging"
    },
    {
      "id": "2",
      "name": "Best Friend Bazaar - Production"
    }
  ]
}
```

You can add, edit and delete accordingly: the primary consideration is that the `id` value for each is unique, as this `id` is used to "tie" the vertical to its API Key, which we store locally in user-secrets.

This set up is described in the next section.

### Set up vertical API Keys

We store API keys separately to the primary vertical config given their sensitive nature. In doing so, we go some way to avoiding the leakage of sensitive config data. Care should still be taken with the storage and use of API Keys at all times.

> **NOTE:** This tool should never be used to create orders on customer production instances.

To configure an API key for each vertical follow these steps.

1. User secrets should already be configured for this project: in the `.csproj` file you should find the following tags in the `<PropertyGroup>` section:

```xml
<UserSecretsId>1a054004-37d1-4aeb-9bd7-4a165573f44b</UserSecretsId>
```

The value represents the name of a folder location on your local file system that contains a json file (`secrets.json`) with the user-secret data. You can amend this id value as you like, but it should be unique for each .NET project adopting user-secrets.

2. Configure Key values

User secrets can be manipulated at the command line, at the time of writing however that approach does not allow you to directly manipulate JSON arrays, which is what we want to adopt to store our keys. Therefore we need to directly manipulate the `secrets.json` file to add the keys for our verticals.

#### Where is the `secrets.json` file?

The location of the `secrets.json` file is OS dependant, however in all instances the file is stored on the local file system in a location accessible only to the logged in user:

- Windows: `%APPDATA%\Microsoft\UserSecrets\{user_secrets_id}\secrets.json`
- Mac: `~/.microsoft/usersecrets/{user_secrets_id}/secrets.json`
- Linux: `~/.microsoft/usersecrets/{user_secrets_id}/secrets.json`

Navigate to the `secrets.json` file and populate key data in the following way, where the `id` is a match for the `id` representing the vertical in `appsettings.json`


```json
{
  "verticalKeys": [
    {
      "id": 1,
      "key": "abcd1234"
    },
    {
      "id": 2,
      "key": "efgh5678"
    }
  ]
}
```

> **Note:** the name of the array holding our keys is called `verticalKeys`.