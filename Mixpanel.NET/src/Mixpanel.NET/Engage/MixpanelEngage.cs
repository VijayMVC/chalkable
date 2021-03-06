using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Mixpanel.NET.Events;

namespace Mixpanel.NET.Engage {
  public class MixpanelEngage : MixpanelClientBase, IEngage {
    private readonly EngageOptions _options;

    /// <summary>
    /// Creates a new Mixpanel Engage client for a given API token
    /// </summary>
    /// <param name="token">The API token for MixPanel</param>
    /// <param name="http">Optional: An implementation of IMixpanelHttp, <see cref="MixpanelHttp"/>
    /// Determines if class names and properties will be serialized to JSON literally.
    /// If false (the default) spaces will be inserted between camel-cased words for improved 
    /// readability on the reporting side.
    /// </param>
    /// <param name="options">Optional: Specific options for the API <see cref="EngageOptions"/></param>
    public MixpanelEngage(string token, IMixpanelHttp http = null, EngageOptions options = null)
      : base(token, http) {
      _options = options ?? new EngageOptions();
    }

    private bool Engage(string distinctId, string ip, IDictionary<string, object> setProperties = null, 
      IDictionary<string, object> incrementProperties = null) {
      // Standardize token and time values for Mixpanel
      var dictionary = 
        new Dictionary<string, object> {{"$token", token}, {"$distinct_id", distinctId} , {"$ip", ip}};

      if (setProperties != null) dictionary.Add("$set", setProperties);

      if (incrementProperties != null) dictionary.Add("$add", incrementProperties);

      var data = new JavaScriptSerializer().Serialize(dictionary);

      var values = "data=" + data.Base64Encode();

      var contents = _options.UseGet
        ? http.Get(Resources.Engage(_options.ProxyUrl), values)
        : http.Post(Resources.Engage(_options.ProxyUrl), values);

      return contents == "1";
    }

    public bool Set(string distinctId, string ip, IDictionary<string, object> setProperties) {
      return Engage(distinctId, ip, setProperties);
    }

    public bool Increment(string distinctId, IDictionary<string, object> incrementProperties) {
      return Engage(distinctId, "", incrementProperties: incrementProperties);
    }
  }
}