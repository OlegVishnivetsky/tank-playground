using Sirenix.OdinInspector.Editor;
using Unity.Netcode;
using UnityEditor; 

namespace TankPlayground.Editor
{
    [CustomEditor(typeof(NetworkBehaviour), true)] 
    public class OdinNetworkBehaviourEditor : OdinEditor { }
}