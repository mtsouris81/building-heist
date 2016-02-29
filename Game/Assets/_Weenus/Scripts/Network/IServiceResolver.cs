using UnityEngine;

namespace Weenus.Network
{
    public interface IServiceResolver
    {
        void StartRequest(string url);
        bool AttemptResolveResponse();
        string Name { get; }
        object Response { get; }
        bool IsActive { get; }
        GameObject WebLoadingDisplay { get; set; }
    }

}
