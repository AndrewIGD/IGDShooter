using System;
using System.Net;
using Mirror.Discovery;
using UnityEngine;

public class MirrorDiscovery : NetworkDiscoveryBase<ServerRequest, MirrorDiscoveryResponse>
{
    #region Server

    [HideInInspector] public Func<MirrorDiscoveryResponse> ServerDiscoveryHandler;
    
    protected override MirrorDiscoveryResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
    {
        Debug.Log("Mirror Discovery");
        
        try
        {
            var response = ServerDiscoveryHandler?.Invoke() ?? new MirrorDiscoveryResponse();
            response.serverId = ServerId;
            response.uri = transport.ServerUri();
            return response;
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
    }

    #endregion

    #region Client
        
    protected override ServerRequest GetRequest() => new ServerRequest();
        
    protected override void ProcessResponse(MirrorDiscoveryResponse response, IPEndPoint endpoint)
    {
        response.EndPoint = endpoint;
            
        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };
            
        response.uri = realUri.Uri;

        OnServerFound.Invoke(response);
    }

    #endregion
}
