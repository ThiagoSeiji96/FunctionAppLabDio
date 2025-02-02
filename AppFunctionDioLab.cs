using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppLabDio
{
    public static class AppFunctionDioLab
    {
        [FunctionName("AppFunctionDioLab")]
         public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Iniciando validação de CPF..");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonSerializer.Deserialize<string>(requestBody);

            if(data is null)
                return new BadRequestObjectResult("Por favor, informe o CPF.");


            if(!ValidaCpf(data))
                return new BadRequestObjectResult("CPF inválido! Informe o CPF.");

            return new OkObjectResult("CPF válido!");
        }

        public bool ValidaCpf(string cpf){
            // Remove qualquer caractere não numérico
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais (CPF como 111.111.111-11)
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Cálculo do primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += (cpf[i] - '0') * (10 - i);
        }
        int digito1 = 11 - (soma % 11);
        if (digito1 >= 10)
            digito1 = 0;

        // Cálculo do segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += (cpf[i] - '0') * (11 - i);
        }
        int digito2 = 11 - (soma % 11);
        if (digito2 >= 10)
            digito2 = 0;

        // Verifica se os dígitos verificadores estão corretos
        return cpf[9] == (char)(digito1 + '0') && cpf[10] == (char)(digito2 + '0');
        }
    }
}
