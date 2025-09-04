using UnityEngine.UIElements;

namespace EZaca.UIElements
{
    public interface IDetachFromPanelHandler
    {
        public void OnDetachFromPanel(DetachFromPanelEvent eventData);
    }
}