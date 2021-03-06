﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiExercCloudComputing.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public string Post([FromBody]string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //conecta na fila f1
                StorageHelper.StorageHelper.ConnectToStorage("fila1");

                //manda o que foi recebido via post para a fila
                StorageHelper.StorageHelper.SendMessageToQueue(value);

                return "Mensagem enviada!"; 
            }else
                return "Erro ao enviar a mensagem!";
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
