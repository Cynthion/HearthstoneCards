using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HearthstoneCards.Helper
{
    public class CardConverter : JsonConverter

{
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var card = new Card();

            // TODO finish json conv
            //card.AccessToken = (string)jo["access_token"];
            //card.RefreshToken = (string)jo["refresh_token"];
            //card.TokenType = (string)jo["token_type"];
            //card.ExpiresIn = (int)jo["expires_in"];
            //card.Scope = (string)jo["scope"];
            //card.CreatedAt = (int)jo["created_at"];
            //card.Error = (string)jo["error"];
            //card.ErrorDescription = (string)jo["error_description"];
            return card;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Card));
        }
}
}
