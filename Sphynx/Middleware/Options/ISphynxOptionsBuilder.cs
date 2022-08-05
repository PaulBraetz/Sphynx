namespace Sphynx.Middleware
{
    public interface ISphynxOptionsBuilder<TOptions>
    {
        TOptions Build();
    }
}
