using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    // [Route("[controller]")]
    public class PersonagensController : Controller
    {
        public string uriBase = "http://www.lucasns06.somee.com/rpgapi/Personagens/";

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // [Route("Personagens")]
        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionIdUsuario")))
                {
                    return RedirectToAction("Sair", "Usuarios");
                }
                string perfil = HttpContext.Session.GetString("SessionPerfilUsuario");
                ViewBag.Perfil = perfil;

                //string uriComplementar = "GetAll";
                int usuarioId = int.Parse(HttpContext.Session.GetString("SessionIdUsuario"));
                string uriComplementar = (perfil == "Admin") ? "GetAll" : $"GetByPerfil/{usuarioId}";


                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listaPersonagens = await Task.Run(() => JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                    return View(listaPersonagens);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateAsync(PersonagemViewModel p)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Personagem {0}, Id {1} salvo com sucesso!", p.Nome, serialized);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Create");
            }
        }
        [HttpGet]
        public async Task<ActionResult> DetailsAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PersonagemViewModel p = await Task.Run(() =>
                    JsonConvert.DeserializeObject<PersonagemViewModel>(serialized));
                    return View(p);
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> EditAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PersonagemViewModel p = await Task.Run(() => JsonConvert.DeserializeObject<PersonagemViewModel>(serialized));
                    return View(p);
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditAsync(PersonagemViewModel p)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Personagem {0}, Id {1} salvo com sucesso!", p.Nome, serialized);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Personagem Id {0} removido com sucesso", id);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> DisputaGeralAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = "https://rpgapilucasns.azurewebsites.net/Personagens/getall";
                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);

                string serialized = await response.Content.ReadAsStringAsync();

                List<PersonagemViewModel> listaPersonagens = await Task.Run(() => JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                string uriDisputa = "https://rpgapilucasns.azurewebsites.net/Disputas/DisputaEmGrupo";
                DisputaViewModel disputa = new DisputaViewModel();
                disputa.ListaIdPersonagens = new List<int>();
                disputa.ListaIdPersonagens.AddRange(listaPersonagens.Select(p => p.Id));

                var content = new StringContent(JsonConvert.SerializeObject(disputa));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await httpClient.PostAsync(uriDisputa, content);

                serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    disputa = await Task.Run(() => JsonConvert.DeserializeObject<DisputaViewModel>(serialized));
                    TempData["Mensagem"] = string.Join("<br/>", disputa.Resultados);
                }
                else
                    throw new System.Exception(serialized);

                return RedirectToAction("Index", "Personagens");
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ZerarRankingRestaurarVidasAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string uriComplementar = "ZerarRankingRestaurarVidas";
                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, null);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "Rankings zerados e vidas dos personagens restauradas com sucesso.";
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            { TempData["MensagemErro"] = ex.Message; }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> RestaurarPontosVidaAsync(int id)
        {
            try
            {
                string uriComplementar = "RestaurarPontosVida";
                PersonagemViewModel p = new PersonagemViewModel();
                p.Id = id;
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario"); httpClient.DefaultRequestHeaders.Authorization = new
               AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p)); content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync(); if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = "Pontos de vida do personagem restaurados com sucesso";
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> ZerarRankingAsync(int id)
        {
            try
            {
                string uriComplementar = "ZerarRanking";
                PersonagemViewModel p = new PersonagemViewModel();
                p.Id = id;
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario"); httpClient.DefaultRequestHeaders.Authorization = new
               AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p)); content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync(); if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = "Ranking do personagem zerado com sucesso";
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}