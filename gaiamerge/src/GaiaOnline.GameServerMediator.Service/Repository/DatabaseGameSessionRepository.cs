﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace GaiaOnline
{
	public sealed class DatabaseGameSessionRepository : IGameSessionRepository
	{
		private GameSessionDatabaseContext GameSessionContext { get; }

		public DatabaseGameSessionRepository(GameSessionDatabaseContext gameSessionContext)
		{
			if(gameSessionContext == null) throw new ArgumentNullException(nameof(gameSessionContext));

			GameSessionContext = gameSessionContext;
		}

		/// <inheritdoc />
		public async Task<bool> HasSession(int userId)
		{
			return await GameSessionContext.GameSessions.AnyAsync(s => s.UserId == userId);
		}

		/// <inheritdoc />
		public async Task<bool> HasSession(Guid sessionGuid)
		{
			return await GameSessionContext.GameSessions.AnyAsync(s => s.SessionGuid == sessionGuid);
		}

		/// <inheritdoc />
		public async Task<GameSessionModel> GetSessionByGuid(Guid sessionGuid)
		{
			//We should assume the callers believes there to be a session under this guid available
			//Since we expose a way to check, HasSession.
			//Although there is a RACE CONDITION here that may occur in the future as this feature expands
			//Supressing exceptions is simplier and more efficient than the pessimistic locking though
			return await GameSessionContext.GameSessions.FirstAsync(s => s.SessionGuid == sessionGuid);
		}

		/// <inheritdoc />
		public async Task<GameSessionModel> GetSessionById(int userId)
		{
			//See above comments
			return await GameSessionContext.GameSessions.FirstAsync(s => s.UserId == userId);
		}

		/// <inheritdoc />
		public async Task<SessionCreationResult> TryCreateSession(int userId, string ipAddress)
		{
			if(await HasSession(userId))
				return new SessionCreationResult();

			//We don't have the database generate a guid, if it even could, and yes this is safe.
			Guid sessionGuid = Guid.NewGuid();

			await GameSessionContext.GameSessions.AddAsync(new GameSessionModel(userId, sessionGuid, ipAddress));

			int rowAmountChanged = 0;
			try
			{
				rowAmountChanged = await GameSessionContext.SaveChangesAsync();
			}
			catch(Exception e) //likely a session was made, due to a race condition, and as a result of our concurrency approach it will throw
			{
				//TODO: Log the message, but it's very likely to be MySqlException with duplicate insert error code
				return new SessionCreationResult();
			}

			//None were added BUT we didn't encounter a throw. Not sure this could ever happen though.
			if(rowAmountChanged == 0)
				return new SessionCreationResult();

			return new SessionCreationResult(sessionGuid);
		}
	}
}
