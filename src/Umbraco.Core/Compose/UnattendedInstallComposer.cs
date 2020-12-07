using Umbraco.Core.Composing;

namespace Umbraco.Core.Compose
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Install, MaxLevel = RuntimeLevel.Install)]
    public class UnattendedInstallComposer : ComponentComposer<UnattendedInstallComponent>
    {
    }
}
