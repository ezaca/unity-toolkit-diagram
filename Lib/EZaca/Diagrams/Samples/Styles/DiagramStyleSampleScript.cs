using EZaca.Diagrams;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DiagramStyleSampleScript : MonoBehaviour
{
    [SerializeField] private VisualElementReference<Button> previousButton;
    [SerializeField] private VisualElementReference<Button> nextButton;
    [SerializeField] private VisualElementReference<DropdownField> styleDropdownReference;
    [SerializeField] private VisualElementReference<DiagramElement> diagramReference;
    [SerializeField] private List<StyleSheet> styleSheets;

    private DropdownField styleDropdown;
    private DiagramElement diagram;

    private void Awake()
    {
        diagramReference.RegisterReferenceResolvedCallback(value => diagram = value);

        styleDropdownReference.RegisterReferenceResolvedCallback(value =>
        {
            styleDropdown = value;
            styleDropdown.RegisterValueChangedCallback(evt => SetStylesheet(styleDropdown.index));
            SetStylesheet(styleDropdown.index);
        });
        
        previousButton.RegisterReferenceResolvedCallback(button =>
        {
            button.RegisterCallback<ClickEvent>(evt =>
            {
                if (styleDropdown.index > 0)
                    styleDropdown.index--;
                else
                    styleDropdown.index = styleDropdown.choices.Count - 1;
            });
        });

        nextButton.RegisterReferenceResolvedCallback(button =>
        {
            button.RegisterCallback<ClickEvent>(evt =>
            {
                if (styleDropdown.index < styleDropdown.choices.Count - 1)
                    styleDropdown.index++;
                else
                    styleDropdown.index = 0;
            });
        });
    }

    private void SetStylesheet(int index)
    {
        diagram.styleSheets.Clear();

        if (styleSheets[index] is not null)
            diagram.styleSheets.Add(styleSheets[index]);

        diagram.RepaintConnections();
    }
}
