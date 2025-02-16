namespace App.Contracts.Object.Base
{
    public interface IPersonApp
    {
        Task<List<PersonView>> personViews();
    }
}
