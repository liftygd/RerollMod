using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using MewgenicsModSdk;
using MewgenicsModSdk.Game;

namespace RerollMod;

class Server()
{
    private HttpClient? _client = null;
    private readonly Encrypter _encrypter = new Encrypter();
    private ServerConfig? _serverConfig;

    public void ActivateClient(ModConfig config)
    {
        if (_client != null)
            return;
        
        var server = config.GetString("server", string.Empty);
        var key = config.GetString("key", string.Empty);

        if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(key))
            return;
        
        var serverConfigString = _encrypter.Decrypt(server, key);
        if (string.IsNullOrEmpty(serverConfigString))
            return;

        _serverConfig = JsonSerializer.Deserialize(
            serverConfigString,
            SourceGenerationContext.Default.ServerConfig);
        
        if (_serverConfig == null ||
            string.IsNullOrEmpty(_serverConfig.IpAddress) ||
            string.IsNullOrEmpty(_serverConfig.ApiKey))
            return;
        
        _client = new HttpClient
        {
            BaseAddress = new Uri(_serverConfig.IpAddress)
        };
        
        _client.DefaultRequestHeaders.Add("X-Api-Key", _serverConfig.ApiKey);
    }

    private string CreateCatStateCall(Guid id, string catName, long catId, string className,
        string spell0, string spell1, string spell2, string spell3,
        string passive0, string passive1, string disorder0, string disorder1)
    {
        string? ToJsonValue(string? value) => string.IsNullOrEmpty(value) ? null : $"\"{value}\"";

        string call = $"{{\"id\":\"{id}\",\"catName\":\"{catName}\",\"catId\":\"{catId}\",\"className\":\"{className}\"," +
                      $"\"abilities\":[{{\"id\":1,\"name\":{ToJsonValue(spell0)}}},{{\"id\":2,\"name\":{ToJsonValue(spell1)}}}," +
                      $"{{\"id\":3,\"name\":{ToJsonValue(spell2)}}},{{\"id\":4,\"name\":{ToJsonValue(spell3)}}}]," +
                      $"\"passives\":[{{\"id\":1,\"name\":{ToJsonValue(passive0)}}},{{\"id\":2,\"name\":{ToJsonValue(passive1)}}}]," +
                      $"\"disorders\":[{{\"id\":1,\"name\":{ToJsonValue(disorder0)}}},{{\"id\":2,\"name\":{ToJsonValue(disorder1)}}}]}}";

        return call;
    }

    public string CreateCatState(Guid id, GameChar cat)
    {
        return CreateCatStateCall(id, cat.Name, cat.CatId, cat.ClassName, cat.Spell0, cat.Spell1, cat.Spell2, cat.Spell3,
            cat.Passive0, cat.Passive1, cat.Disorder0, cat.Disorder1);
    }

    public void UpdateCat(string json)
    {
        if (_client == null || _serverConfig == null)
            return;
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var task = _client.PostAsync("api/cat/update", content);
    }

    public void EndRun(Guid playerId)
    {
        if (_client == null  || _serverConfig == null)
            return;
        
        var content = new StringContent($"{{\"id\":\"{playerId}\"}}", Encoding.UTF8, "application/json");
        var task = _client.PostAsync("api/run/end", content);
    }

    public void RollCat(Guid playerId, GameChar cat)
    {
        if (_client == null  || _serverConfig == null)
            return;
        
        var content = new StringContent($"{{\"id\":\"{playerId}\"," +
                                        $"\"catName\":\"{cat.Name}\"," +
                                        $"\"catId\":\"{cat.CatId}\"," +
                                        $"\"className\":\"{cat.ClassName}\"}}", Encoding.UTF8, "application/json");
        
        var task = _client.PostAsync("api/cat/roll", content);
    }
}