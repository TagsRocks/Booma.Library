﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Booma
{
	public static class CharacterNameValidator
	{
		/// <summary>
		/// Indicates if the provided character name is valid.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool isNameValid(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return false;

			//We don't support the bullshit unicode or any of the nonsense ASCII
			//DO NOT USE char.IsDigit or anything. It will accept weird spanish letters/etc.
			if(!Regex.IsMatch(name, @"^[a-zA-Z0-9]*$"))
				return false;

			return true;
		}
	}
}
