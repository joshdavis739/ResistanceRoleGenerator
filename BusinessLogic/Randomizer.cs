using System.Security.Cryptography;

namespace BusinessLogic
{
    public class Randomizer<T>
    {
        public virtual IOrderedEnumerable<T> Randomize(ICollection<T> collection) =>
            collection.OrderBy(x => RandomNumberGenerator.GetInt32(int.MaxValue));
    }
}
