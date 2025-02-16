using App.Object.Base;
using Domain.Objects.Base;


namespace ConfApp.Rep
{
    public class PersonRep : BaseRep<Person, int>, IPersonRep
    {
        public PersonRep(MyContext context) : base(context)
        {

        }
    }
}
