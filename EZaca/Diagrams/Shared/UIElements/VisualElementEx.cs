using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.UIElements
{
    public abstract class VisualElementEx : VisualElement
    {
        protected VisualElementEx()
        {
            if (this is IAttachToPanelHandler attachToPanel)
                RegisterCallback<AttachToPanelEvent>(attachToPanel.OnAttachToPanel);

            if (this is IDetachFromPanelHandler detachFromPanel)
                RegisterCallback<DetachFromPanelEvent>(detachFromPanel.OnDetachFromPanel);
        }

        public virtual void Repaint()
        {
            MarkDirtyRepaint();
        }

        protected T SearchBottomUp<T>() where T : class
        {
            return SearchParent<T>(this);
        }

        public static T SearchParent<T>(VisualElement top) where T: class
        {
            VisualElement current = top;
            while (current is not null)
            {
                if (current is T t)
                    return t;
                current = current.parent;
            }
            return null;
        }
    }
}