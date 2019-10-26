using System.Collections.Generic;
using Youtube_DL.UiServer.Options;

namespace Youtube_DL.UiServer.Configuration
{
	/// <summary>	The status code options. </summary>
	[ConfigurationName(name: "StatusCode")]
	public class StatusCodeOptions
	{
		/// <summary>	Gets or sets the self handled codes. </summary>
		/// <value>	The self handled codes. </value>
		public List<int> SelfHandledCodes { get; set; }
	}
}