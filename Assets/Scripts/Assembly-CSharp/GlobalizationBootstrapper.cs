using System.Globalization;
using System.Threading;
using UnityEngine;

public static class GlobalizationBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ForceInvariantCulture()
    {
        CultureInfo invariant = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = invariant;
        CultureInfo.DefaultThreadCurrentUICulture = invariant;
        Thread.CurrentThread.CurrentCulture = invariant;
        Thread.CurrentThread.CurrentUICulture = invariant;
    }
}