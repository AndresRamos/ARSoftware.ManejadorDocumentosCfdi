﻿using System.Windows;
using System.Windows.Controls;

namespace Presentation.WpfApp.Helpers;

public class PasswordHelper
{
    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

    public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password",
        typeof(string),
        typeof(PasswordHelper),
        new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper));

    public static bool GetAttach(DependencyObject dp)
    {
        return (bool)dp.GetValue(AttachProperty);
    }

    public static string GetPassword(DependencyObject dp)
    {
        return (string)dp.GetValue(PasswordProperty);
    }

    public static void SetAttach(DependencyObject dp, bool value)
    {
        dp.SetValue(AttachProperty, value);
    }

    public static void SetPassword(DependencyObject dp, string value)
    {
        dp.SetValue(PasswordProperty, value);
    }

    private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;

        if (passwordBox == null)
            return;

        if ((bool)e.OldValue)
            passwordBox.PasswordChanged -= PasswordChanged;

        if ((bool)e.NewValue)
            passwordBox.PasswordChanged += PasswordChanged;
    }

    private static bool GetIsUpdating(DependencyObject dp)
    {
        return (bool)dp.GetValue(IsUpdatingProperty);
    }

    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;

        if (passwordBox is null)
            throw new ArgumentException("Sender is not a PasswordBox");

        passwordBox.PasswordChanged -= PasswordChanged;

        if (!GetIsUpdating(passwordBox))
            passwordBox.Password = (string)e.NewValue;

        passwordBox.PasswordChanged += PasswordChanged;
    }

    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        if (passwordBox is null)
            throw new ArgumentException("Sender is not a PasswordBox");

        SetIsUpdating(passwordBox, true);
        SetPassword(passwordBox, passwordBox.Password);
        SetIsUpdating(passwordBox, false);
    }

    private static void SetIsUpdating(DependencyObject dp, bool value)
    {
        dp.SetValue(IsUpdatingProperty, value);
    }
}
