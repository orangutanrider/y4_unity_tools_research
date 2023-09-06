using UnityEditor;
using UnityEngine;
using System;

public static class PropertyAlertGUI
{
    static bool Initialized { get; set; } = false;
    static Texture ErrorIcon = null;
    static Texture WarningIcon = null;
    static Texture InfoIcon = null;

    public enum AlertType
    {
        Error,
        Warning,
        Info
    }

    public static bool BeginFoldoutHeaderGroup(bool foldout, string content, AlertType alertType, GUIStyle style = null, Action<Rect> menuAction = null, GUIStyle menuIcon = null)
    {
        // create a function for this or change the strings to use constants in refactor
        // Load textures for the first time
        if (Initialized == false)
        {
            Initialized = true;
            ErrorIcon = Resources.Load<Texture>("errorIcon");
            WarningIcon = Resources.Load<Texture>("warningIcon");
            InfoIcon = Resources.Load<Texture>("infoIcon");
        }

        // Create a function for this in refactor
        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        GUIContent guiContent = new GUIContent(content, alertIcon);

        return EditorGUILayout.BeginFoldoutHeaderGroup(foldout, guiContent, style, menuAction, menuIcon);
    }

    public static GUIContent CreateAlertGUIContentFromProperty(SerializedProperty serializedProperty, AlertType alertType) // could use this as a simple extension method in place of all of these
    {
        // create a function for this or change the strings to use constants in refactor
        // Load textures for the first time
        if (Initialized == false)
        {
            Initialized = true;
            ErrorIcon = Resources.Load<Texture>("errorIcon");
            WarningIcon = Resources.Load<Texture>("warningIcon");
            InfoIcon = Resources.Load<Texture>("infoIcon");
        }

        // Create a function for this in refactor
        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        return new GUIContent(serializedProperty.displayName, alertIcon, serializedProperty.tooltip);
    }

    // don't re-use this naming convention
    public static bool ManualDrawAlertProperty(Rect position , SerializedProperty serializedProperty, GUIContent content, AlertType alertType)
    {
        // Load textures for the first time
        if (Initialized == false)
        {
            Initialized = true;
            ErrorIcon = Resources.Load<Texture>("errorIcon");
            WarningIcon = Resources.Load<Texture>("warningIcon");
            InfoIcon = Resources.Load<Texture>("infoIcon");
        }

        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        // create content
        content.image = alertIcon;

        // create property field
        return EditorGUI.PropertyField(position,serializedProperty, content);
    }

    #region LayoutDraw

    // don't re-use these naming conventions
    // make it more like Unity's
    public static bool DrawAlertProperty(SerializedProperty serializedProperty, AlertType alertType, params GUILayoutOption[] options)
    {
        // Load textures for the first time
        if(Initialized == false)
        {
            Initialized = true;
            ErrorIcon = Resources.Load<Texture>("errorIcon");
            WarningIcon = Resources.Load<Texture>("warningIcon");
            InfoIcon = Resources.Load<Texture>("infoIcon");
        }

        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        // create content
        GUIContent content = new GUIContent(serializedProperty.displayName, alertIcon, serializedProperty.tooltip);

        // create property field
        return EditorGUILayout.PropertyField(serializedProperty, content, options);
    }

    public static bool DrawAlertProperty(SerializedProperty serializedProperty, bool includeChildren, AlertType alertType, params GUILayoutOption[] options)
    {
        // Load textures for the first time
        if (Initialized == false)
        {
            Initialized = true;
            ErrorIcon = (Texture)Resources.Load("");
            WarningIcon = (Texture)Resources.Load("");
            InfoIcon = (Texture)Resources.Load("");
        }

        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        // create content
        GUIContent content = new GUIContent(serializedProperty.displayName, alertIcon, serializedProperty.tooltip);

        // create property field
        return EditorGUILayout.PropertyField(serializedProperty, content, includeChildren, options);
    }

    public static bool DrawAlertProperty(SerializedProperty serializedProperty, GUIContent content, AlertType alertType, params GUILayoutOption[] options)
    {
        // Load textures for the first time
        if (Initialized == false)
        {
            Initialized = true;
            ErrorIcon = (Texture)Resources.Load("");
            WarningIcon = (Texture)Resources.Load("");
            InfoIcon = (Texture)Resources.Load("");
        }

        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        // add alert to content
        content.image = alertIcon;

        // create property field
        return EditorGUILayout.PropertyField(serializedProperty, content, options);
    }

    public static bool DrawAlertProperty(SerializedProperty serializedProperty, GUIContent content, bool includeChildren, AlertType alertType, params GUILayoutOption[] options)
    {
        // Load textures for the first time
        if (Initialized == false)
        {
            Initialized = true;
            ErrorIcon = (Texture)Resources.Load("");
            WarningIcon = (Texture)Resources.Load("");
            InfoIcon = (Texture)Resources.Load("");
        }

        // Select correct icon
        Texture alertIcon = null;
        switch (alertType)
        {
            case AlertType.Error:
                alertIcon = ErrorIcon;
                break;
            case AlertType.Warning:
                alertIcon = WarningIcon;
                break;
            case AlertType.Info:
                alertIcon = InfoIcon;
                break;
        }

        // add alert to content
        content.image = alertIcon;

        // create property field
        return EditorGUILayout.PropertyField(serializedProperty, content, includeChildren, options);
    }
    #endregion
}
