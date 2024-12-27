using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    // [Route("[controller]")]
    public class UsuariosController : Controller
    {
        public string uriBase = "http://www.lucasns06.somee.com/rpgapi/Usuarios/";

        [HttpGet]
        public ActionResult Index()
        {
            return View("CadastrarUsuario");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpa toda a sessão do usuário
            TempData["Mensagem"] = "Você saiu com sucesso.";
            return RedirectToAction("IndexLogin", "Usuarios");
        }

        [HttpGet]
        public ActionResult Cadastro()
        {
            return View("CadastrarUsuario");
        }
        [HttpPost]
        public async Task<ActionResult> RegistrarAsync(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Registrar";

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] =
                        string.Format("Usuário {0} Registrado com sucesso! Faça o login para acessar.", u.Username);
                    return View("AutenticarUsuario");
                }
                else
                {
                    throw new System.Exception(serialized + "Erro");
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("index");
            }
        }
        [HttpGet]
        public ActionResult IndexLogin()
        {
            return View("AutenticarUsuario");
        }
        [HttpPost]
        public async Task<ActionResult> AutenticarAsync(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Autenticar";

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UsuarioViewModel uLogado = JsonConvert.DeserializeObject<UsuarioViewModel>(serialized);

                    HttpContext.Session.SetString("SessionTokenUsuario", uLogado.Token);
                    HttpContext.Session.SetString("SessionUsername", uLogado.Username);    

                    HttpContext.Session.SetString("SessionPerfilUsuario", uLogado.Perfil);
                    HttpContext.Session.SetString("SessionIdUsuario", uLogado.Id.ToString());

                     if(uLogado.Foto != null)
                        HttpContext.Session.Set("SessionFotoUsuario", uLogado.Foto);
                    
                    TempData["Mensagem"] = string.Format("Bem-vindo {0}!!!", uLogado.Username);
                    return RedirectToAction("Index", "Personagens");
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return IndexLogin();
            }
        }
        [HttpGet]
        public async Task<ActionResult> IndexInformacoesAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string login = HttpContext.Session.GetString("SessionUsername"); string uriComplementar =
                $"GetByLogin/{login}";
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync(); if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UsuarioViewModel u = await Task.Run(() =>
                   JsonConvert.DeserializeObject<UsuarioViewModel>(serialized));
                    return View(u);
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
        public async Task<ActionResult> AlterarEmail(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriComplementar = "AtualizarEmail";
                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "E-mail alterado com sucesso.";
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;

            }
            return RedirectToAction("IndexInformacoes");
        }

        [HttpGet]
        public async Task<ActionResult> ObterDadosAlteracaoSenha()
        {
            UsuarioViewModel viewModel = new UsuarioViewModel();
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string login = HttpContext.Session.GetString("SessionUsername");
                string uriComplementar = $"GetByLogin/{login}";
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();

                TempData["TituloModalExterno"] = "Alteração de Senha";

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    viewModel = await Task.Run(() => JsonConvert.DeserializeObject<UsuarioViewModel>(serialized));
                    return PartialView("_AlteracaoSenha", viewModel);
                }
                else
                    throw new System.Exception(serialized);

            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("IndexInformacoes");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AlterarSenha(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriComplementar = "AlterarSenha";
                u.Username = HttpContext.Session.GetString("SessionUsername");

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string mensagem = "Senha alterada com sucesso.";
                    TempData["Mensagem"] = mensagem; 
                    return Json(mensagem); 
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Sair()
        {
            try
            {
                HttpContext.Session.Remove("SessionTokenUsuario");
                HttpContext.Session.Remove("SessionUsername");
                HttpContext.Session.Remove("SessionPerfilUsuario");
                HttpContext.Session.Remove("SessionIdUsuario");
                HttpContext.Session.Remove("SessionFotoUsuario");


                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("IndexInformacoes");
            }
        }



    }
}