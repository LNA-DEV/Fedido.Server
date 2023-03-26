using CommonExtensions;
using Fedodo.BE.ActivityPub.Model;
using Fedodo.BE.ActivityPub.Model.ActivityPub;
using Fedodo.BE.ActivityPub.Model.NodeInfo;
using Fedodo.NuGet.Common.Constants;
using Fedodo.NuGet.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fedodo.BE.ActivityPub.Controllers;

public class NodeInfoController : ControllerBase
{
    private readonly ILogger<NodeInfoController> _logger;
    private readonly IMongoDbRepository _repository;

    public NodeInfoController(ILogger<NodeInfoController> logger, IMongoDbRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet(".well-known/nodeinfo")]
    public ActionResult<WebLink> GetNodeInfoLink()
    {
        _logger.LogTrace($"Entered {nameof(GetNodeInfoLink)} in {nameof(NodeInfoController)}");

        var wrapper = new
        {
            links = new List<NodeLink>
            {
                new()
                {
                    Rel = "http://nodeinfo.diaspora.software/ns/schema/2.0",
                    Href = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/nodeinfo/2.0")
                }
            }
        };

        return Ok(wrapper);
    }

    [HttpGet("nodeinfo/2.0")]
    public async Task<ActionResult<NodeInfo>> GetNodeInfo()
    {
        _logger.LogTrace($"Entered {nameof(GetNodeInfo)} in {nameof(NodeInfoController)}");

        var version = Environment.GetEnvironmentVariable("VERSION") ?? "0.0.0";

        var nodeInfo = new NodeInfo
        {
            Version = "2.0",
            Software = new Software
            {
                Name = "Fedodo",
                Version = version
            },
            Protocols = new[]
            {
                "activitypub"
            },
            Services = new Services
            {
                Outbound = Array.Empty<object>(),
                Inbound = Array.Empty<object>()
            },
            Usage = new Usage
            {
                LocalPosts =
                    await _repository.CountAll<Activity>(DatabaseLocations.OutboxCreate.Database,
                        DatabaseLocations.OutboxCreate.Collection) + await _repository.CountAll<Activity>(
                        DatabaseLocations.OutboxAnnounce.Database, DatabaseLocations.OutboxAnnounce.Collection),
                LocalComments = 0, // TODO
                Users = new Users
                {
                    ActiveHalfyear = 1, // TODO
                    ActiveMonth = 1, // TODO
                    Total = await _repository.CountAll<Activity>(DatabaseLocations.Actors.Database,
                        DatabaseLocations.Actors.Collection)
                }
            },
            OpenRegistrations = true,
            Metadata = new Dictionary<string, string>()
        };

        return Ok(nodeInfo);
    }
}