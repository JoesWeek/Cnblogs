namespace Cnblogs.Droid.UI.Listeners
{
    public interface IOnQueryChangeListener
    {
        bool OnQueryTextSubmit(string query);

        bool OnQueryTextChange(string newText);
    }
}