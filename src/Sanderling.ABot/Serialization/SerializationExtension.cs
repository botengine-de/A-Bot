using BotEngine;
using System;
using System.Collections.Generic;

namespace Sanderling.ABot.Serialization
{
	public class Deserialization
	{
		public string Serial;

		public Exception Exception;
	}

	static public class SerializationExtension
	{
		static public KeyValuePair<Deserialization, T> DeserializeIfDifferent<T>(
			this string newValueSerial,
			KeyValuePair<Deserialization, T> oldValue)
		{
			if (newValueSerial == oldValue.Key?.Serial)
				return oldValue;

			Exception exception = null;
			T newValueStruct = default(T);

			try
			{
				if (null != newValueSerial)
					newValueStruct = newValueSerial.DeserializeFromString<T>();
			}
			catch (Exception e)
			{
				exception = e;
			}

			return new KeyValuePair<Deserialization, T>(new Deserialization { Serial = newValueSerial, Exception = exception }, newValueStruct);
		}
	}
}
