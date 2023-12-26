using BlackHole.Core;
using BlackHole.CoreSupport;
using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    public static class BHPreloadExtensions
    {
        /// <summary>
        /// Stores the Joins Data with the DTO Class as Identifier
        /// and then you can execute them as many times as you want
        /// using the 'IBlackHoleViewStorage' Interface.
        /// <para><b>Benefit</b> : With this method, the program doesn't have to calculate the
        /// Joins Data multiple times and it executes the Joins faster.</para>
        /// <para><b>Tip</b> : This method is recommended if the parameters in the current
        /// Joins Data are not depending on the user's inputs.
        /// Run your Joins Once in the StartUp of your program and store them
        /// as Views.</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The index of this Joins Data in the Stored Views List</returns>
        internal static void StoreAsView<Dto>(this StoredViewComplete<Dto> data) where Dto : BHDtoIdentifier
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (existingJoin != null)
            {
                BlackHoleViews.Stored.Remove(existingJoin);
            }

            BlackHoleViews.Stored.Add(data.Data);
        }
    }
}
