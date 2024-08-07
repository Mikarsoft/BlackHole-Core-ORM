﻿using BlackHole.Identifiers;

namespace BlackHole.Core
{
    /// <summary>
    /// An Interface that contains methods to let you get the
    /// stored Views of BlackHole
    /// </summary>
    public interface IBHViews
    {
        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        List<Dto> ExecuteView<Dto>() where Dto : BHDtoIdentifier;

        /// <summary>
        /// <b>Transaction.</b> Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        List<Dto> ExecuteView<Dto>(IBHTransaction transaction) where Dto : BHDtoIdentifier;

        /// <summary>
        /// <b>Asyncronous.</b> Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        Task<List<Dto>> ExecuteViewAsync<Dto>() where Dto : BHDtoIdentifier;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction.</b> Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        Task<List<Dto>> ExecuteViewAsync<Dto>(IBHTransaction transaction) where Dto : BHDtoIdentifier;

        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        List<Dto> ExecuteView<Dto>(Action<BHOrderBy<Dto>> orderBy) where Dto : BHDtoIdentifier;

        /// <summary>
        /// <b>Transaction.</b> Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        List<Dto> ExecuteView<Dto>(Action<BHOrderBy<Dto>> orderBy, IBHTransaction transaction) where Dto : BHDtoIdentifier;

        /// <summary>
        /// <b>Asyncronous.</b> Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        Task<List<Dto>> ExecuteViewAsync<Dto>(Action<BHOrderBy<Dto>> orderBy) where Dto : BHDtoIdentifier;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction.</b> Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty List
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>List of the DTO</returns>
        Task<List<Dto>> ExecuteViewAsync<Dto>(Action<BHOrderBy<Dto>> orderBy,IBHTransaction transaction) where Dto : BHDtoIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        IBHJoinsProcess<Dto> StartJoinUsing<Dto>() where Dto : class, BHDtoIdentifier;
    }
}
