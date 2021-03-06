﻿namespace DreamFactory.Api
{
    using DreamFactory.Model.Database;
    using DreamFactory.Model.System.User;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Represents /system/user API.
    /// </summary>
    public interface ISystemUserApi
    {
        /// <summary>
        /// Retrieve one or more users.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <returns>List of users.</returns>
        Task<IEnumerable<UserResponse>> GetUsersAsync(SqlQuery query);

        /// <summary>
        /// Create one or more users.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="users">Users to create.</param>
        /// <returns>List of created users.</returns>
        Task<IEnumerable<UserResponse>> CreateUsersAsync(SqlQuery query, params UserRequest[] users);

        /// <summary>
        /// Update one or more users.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="users">Users to update.</param>
        /// <returns>List of updated users.</returns>
        Task<IEnumerable<UserResponse>> UpdateUsersAsync(SqlQuery query, params UserRequest[] users);

        /// <summary>
        /// Delete one or more users.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        /// <param name="ids">User IDs to delete.</param>
        /// <returns>By default, only the id property of the record deleted is returned on success. Use 'fields' and 'related' to return more properties of the deleted records.</returns>
        Task<IEnumerable<UserResponse>> DeleteUsersAsync(SqlQuery query, params int[] ids);
    }
}
