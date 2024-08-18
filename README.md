# Sensor Monitoring API

Projeto de implementação de API para monitoramento de ambientes e equipamentos, "conforme o teste DEV Arch" recebendo dados de sensores e armazenando-os em um banco de dados MySQL.

## Estrutura do Projeto

- **APIs**: Exposições de enpoint para executar a persistencias dos dados
- **MySQL**: Banco de dados usado para persistir as informações recebidas.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [MySQL](https://dev.mysql.com/downloads/mysql/)

## Configuração

1. **Clone o repositório**:

```bash
   git clone https://github.com/CezarMR/TesteDevArch.git
   cd TesteDevArch
```

## Configuração

### Configurar o MySQL:

Certifique-se de que o MySQL está instalado e rodando no ambiente. Configure as seguintes credenciais no arquivo **appsettings.json** conforme a configuração para a execução do teste:

## Informações para configurar:
- **Servidor**: localhost
- **Usuário**: root
- **Senha**: sql123
- **Banco de Dados**: SensorMonitoringDb

### Instalações:

Após clonar o repositório, execute os comandos para instalar as dependências, configuração o banco de dados e executar o codigo:
```bash
## Instalar dependências
dotnet restore

## Execute o build do projeto
dotnet build --configuration Release
## ou
dotnet build 

## Configurar o banco de dados e instalar as dependências
dotnet ef migrations add InitialCreate --project SensorMonitoring.API/SensorMonitoring.API.csproj
dotnet ef database update --project SensorMonitoring.API/SensorMonitoring.API.csproj

## Executar o projeto
dotnet run --configuration Release --project SensorMonitoring.API/SensorMonitoring.API.csproj

```

### Acessar o Swagger:
Abra o navegador e navegue para https://localhost:{port} "conforme a porta indicada na execução" para acessar a documentação interativa do Swagger.
Exemplo: http://localhost:5141

## Documentação da API
A API possui dois endpoints principais:

- **POST** /api/sensor-data: Recebe dados de sensores.
- **GET** /api/sensor-data: Retorna todos os dados de sensores armazenados.

#### RESPOSTAS
### PARTE 1:

## Resposta A:
Criei duas APIs principais:

POST /api/SensorData/sensor-data: Esta API recebe dados de sensores. Permite o envio de múltiplos dados em uma única requisição, o que é ideal para cenários onde o sensor acumula dados devido a intermitências na rede.

GET /api/SensorData/sensor-data: Esta API permite a consulta de todos os dados de sensores armazenados, proporcionando uma maneira simples de acessar as medições para análise ou relatórios.

## Resposta B:
A classe de request foi definido como uma classe SensorData:
public class SensorData
{
    public int Id { get; set; }
    public string Codigo { get; set; }
    public DateTimeOffset DataHoraMedicao { get; set; }
    public decimal Medicao { get; set; }
    public int? SensorId { get; set; }
    public Sensor Sensor { get; set; } 
}
Essa classe é utilizada para receber os dados enviados pelo sensor por requisição POST.

## Resposta C:
A opção por usar o MySQL como banco de dados relacional. A escolha do MySQL se justifica por ser um banco de dados robusto e amplamente suportado, ideal para cenários onde:

- Consistência dos dados é crucial, especialmente com medições temporais.
- Consultas e Relatórios: Bancos de dados relacionais são ótimos para executar consultas SQL complexas, facilitando a geração de relatórios.
- Escalabilidade: MySQL oferece várias opções para escalar a aplicação conforme o volume de dados cresce.
- Custo zero, sem licença para o uso do banco de dados.

#### RESPOSTAS
### PARTE 2:

## Resposta A:
API para Vincular um Setor/Equipamento a um Sensor:
O novo endpoint permitir que os usuários vinculem um sensor específico a um determinado setor ou equipamento. O objetivo é estabelecer uma relação entre os sensores e as entidades que eles monitoram, facilitando a organização e consulta dos dados coletados.

- Endpoint: POST /api/SetorEquipamento/vincular-sensor
- Request: O corpo da requisição (VinculacaoRequest) inclui o ID do Setor/Equipamento e o código do sensor que se deseja vincular.
- Response: Retorna um status de sucesso ou uma mensagem de erro se o Setor/Equipamento não for encontrado.

## Resposta B:
API para Retornar as Últimas 10 Medições de Cada Sensor Vinculado a um Setor/Equipamento:
Esta API foi desenvolvida para recuperar as últimas 10 medições de cada sensor que está vinculado a um setor ou equipamento específico. Isso é útil para monitoramento em tempo real ou análise de tendências recentes nos dados coletados por sensores.

- Endpoint: GET /api/SetorEquipamento/{id}/ultimas-medicoes
- Request: O ID do Setor/Equipamento é passado como parte da URL.
- Response: Retorna uma lista de sensores com suas últimas 10 medições.

#### RESPOSTAS
### PARTE 3:

## Resposta A:
Para resolver o problema de detectar medições e notificar o cliente via e-mail, eu implementaria um serviço de background (BackgroundService) em C# usando o .NET. Esse serviço executaria periodicamente a lógica de verificação das medições dos sensores, identificando padrões que indicam um possível problema.

## Resposta B:
Codigo criado para a resposta: AlertService.cs

## Resposta C:
# Teste de 5 Medições Consecutivas Fora do Limite: (Metodo Teste_5_Medicoes_Consecutivas_Fora_Do_Limite)
o teste simula um cenário onde as últimas 5 medições de um sensor estão fora dos limites aceitáveis e verifica se o sistema envia o alerta corretamente.

# Teste da Média das Últimas 50 Medições na Margem de Erro: (Metodo Teste_Media_Das_Ultimas_50_Medicoes_Na_Margem_De_Erro)
O teste simula um cenário em que a média das últimas 50 medições de um sensor está dentro da margem de erro, verificando se o sistema envia um alerta de atenção corretamente.

#### RESPOSTAS
### PARTE 4:
Existem várias abordagens que podemos adotar para resolver ou mitigar esse problema. Aqui combinaria o uso de três abordagens o uso de filas, processamento em lote e o uso de cachamento.
Segue o detalhes para essas soluções e seus benefícios:

## 1. Filas de Mensagens (Message Queues)
# Solução:
Implementar uma arquitetura baseada em filas de mensagens usando tecnologias como RabbitMQ, Apache Kafka, ou Azure Service Bus.
Em vez de processar os dados dos sensores diretamente na aplicação web, os dados seriam colocados em uma fila. Um ou mais consumidores então processariam esses dados assincronamente.
# Benefícios:
A fila desacopla o recebimento dos dados do processamento, permitindo que o sistema lide melhor com picos de carga.
As mensagens podem ser processadas em paralelo por múltiplos consumidores, aumentando a capacidade de processamento.

## 2. Processamento em Lote (Batch Processing)
# Solução:
Em vez de processar os dados dos sensores em tempo real, podemos acumular os dados em pequenos lotes e processá-los periodicamente ou na queda de conexão de internet.
Isso pode ser combinado com o uso de filas para gerenciar os lotes de forma mais eficaz.
# Benefícios:
Reduz a sobrecarga de processamento em tempo real e permite otimizar o uso dos recursos de computação.

## 3. Cacheamento
# Solução:
Implementar cacheamento para armazenar temporariamente os dados que são acessados com frequência, utilizando tecnologias como Redis ou Memcached.
Cacheamento pode ser aplicado tanto para dados estáticos quanto para resultados de consultas frequentes.
# Benefícios:
Reduz a carga no banco de dados e melhora a performance de leitura, já que os dados podem ser servidos diretamente do cache.

