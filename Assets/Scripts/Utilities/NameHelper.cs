using NUnit.Framework;
using System.Collections.Generic;

public static class NameHelper
{
    public static string GetRandomName()
    {
        var firstNameIndex = UnityEngine.Random.Range(0, FirstNames.Count);
        var lastNameIndex = UnityEngine.Random.Range(0, LastNames.Count);
        var titleIndex = UnityEngine.Random.Range(0, Titles.Count);

        var firstName = FirstNames[firstNameIndex];
        var lastName = LastNames[lastNameIndex];
        var title = Titles[titleIndex];

        var hasTitle = UnityEngine.Random.Range(0f, 1f) < .25f ? true : false;

        return hasTitle
            ? $"{firstName} '{title}' {lastName}"
            : $"{firstName} {lastName}";
    }

    private static List<string> FirstNames { get; } = new()
    {
        "Sorbreena",
        "Jrason",
        "Cronnor",
        "Clameron",
        "Kripley",
        "Krundugulous",
        "Lappy",
        "Trip",
        "Alphred",
        "Tran",
        "Ploringo",
        "Dropkick",
        "Clip",
        "Kantokul",
        "Bap"
    };

    private static List<string> Titles { get; } = new()
    {
        "The All-Knowing",
        "Master of Shades",
        "The Dumb",
        "Bastard",
        "Longlost",
        "The Kid"
    };

    private static List<string> LastNames { get; } = new()
    {
        "Lampshade",
        "Dingo",
        "Desk",
        "Everyman",
        "Suhmith",
        "McTrilgo",
        "Dranky",
        "Assmast",
        "Murfy",
        "Bippity",
        "Plorbus",
        "Sreen",
        "Grildo",
        "Newmman"
    };
}