# Marketplay
Venha testar seus conhecimentos de Libras enquanto faz compras em um supermercado virtual. Você consegue acertar todos os produtos?
A VERSÃO ATUAL DO JOGO NO MOMENTO NÃO REFLETE A FINAL DISPONÍVEL NO WEBSITE
<img width="1194" height="668" alt="image" src="https://github.com/user-attachments/assets/8b1f156c-9259-4c91-ad6c-dd251ef55c3c" />

# Guia para programação
Abaixo estão algumas indicações de como deve ser escrito o código com a finalidade de garantir clareza, eficiência e organização. Ajude na legibilidade do código que muito provávelmente terá de ser alterado no futuro.

VERSÃO DO UNITY: 2022.3.62f3 [LTS]

## Dicas gerais
* Utilize funções pontuais que atinjam um objetivo consistentemente: o requisito pode mudar, mas com um código bem preparado você poderá alterar apenas parâmetros para atingir um novo objetivo, sem necessidade de reescrever o código.
* Organize as pastas de forma a ser fácil de encontrar quais assets e scripts são usados em cada cena.
* Siga padrões de nomenclatura e use nomes claros para variáveis e métodos, valorizando mais uma função que execute uma tarefa do que um código monolítico com comentários.
* No caso de um código não estar mais sendo utilizado, retire-o ou comente-o. Evitando códigos redundantes achar bugs é muito mais direto.
* Escreva todo o código (nomes de variáveis, funções, classes, etc.) e comentários em inglês.

## Layout de arquivo
Sempre organize seu arquivo em seções de acordo com a tabela abaixo
| <b>Seção</b> | <b>Linha vazia entre membros</b> |
| :---: | :---: |
| using directive | Não usar |
| Eventos | Opcional |
| SerializeFields | Opcional |
| Variaveis | Opcional
| Properties | Opcional
| Métodos/Funções | Obrigatório

## Naming Conventions
Para maioria dos casos usaremos naming convention <b>PascalCase</b>. Abaixo há uma tabela com exceções a isto:
| <b>Tipo</b> | <b>Convention</b> |
| :---: | :---: |
| Private fields | _camelCase |
| Protected fields | PascalCase |
| const fields | PascalCase
| Variáveis de métodos/funções | camelCase |
| Eventos | OnPascalCase |
| Interfaces | IPascalCase |
| Argumentos genéricos | TPascalCase

## Nomeação de eventos
Para eventos, sempre inicie com o prefixo <b>On</b>. Para o resto, use <b>pretérito</b> para eventos que são chamados ao final da lógica, como abaixo:
```cs
class Player
{
    public event Action OnDied;   // Past tense (pretérito)
    
    public void Die()
    {
        // Dying logic...
        OnDied?.Invoke();       // Chamado ao final da lógica de Dying
    }
}
```
Use <b>presente simples</b> para eventos que são chamados anteriormente na lógica, como abaixo:
```cs
class Player
{
    public event Action OnDie;   // Presente simples
    
    public void Die()
    {
        OnDie?.Invoke();         // Evento chamado antes da lógica de dying
        // Dying logic...
    }
}
```

## Nomeações de manipuladores de eventos (Event Handlers)
Para métodos que manipulam eventos particulares, use o prefixo <b>Handle</b>, <b>NÃO USE O PREFIXO On</b>. Exemplo:

```cs
class SomeUI
{
    public void Setup(Player player)
    {
        player.OnDied += HandlePlayerDied;    // Handler method starts with Handle
    }
    
    private void HandlePlayerDied() { }
}
```

## Namespaces
Uso de namespaces é recomendado no projeto a fim de facilitar a visualização de dependências, e evitar conflitos com classes que podem ter o mesmo nome, porém estarem presentes em namespaces distintas. Ao usar namespace, garanta que o seu nome corresponda ao caminho das pastas. Exemplo: um script CharacterController.cs localizado na pasta Scripts/Game/Character deve ter a seguinte namespace:
```cs
namespace Game.Character // corresponde ao caminho nas pastas
{
  public class CharacterController
  {
  }
}
```

## Variáveis e declarações
Sempre atribua variáveis como privadas nas classes. Modificar o valor de uma variável fora do escopo da classe torna o processo de debug difícil. Se for necessário acessar o valor dessa variável externamente, use o recurso de Properties do C#. Se for necessário alterar o valor da variável externamente, configure o ```set``` da property.
```cs
private int _sum;
public int Sum { get { return _sum; } set { _sum = value; } }
```
Somente acesso de leitura, sem poder escrever:
```cs
private int _sum;
public int Sum => _sum;
```

Não declare múltiplas variáveis em uma mesma linha. Isso pode causar complicações no versionamento e controle do git, além de tornar a legibilidade ruim.

```cs
// Incorreto
private int _a, _b, _c;   

// Correto
private int _a;
private int _b;
private int _c;
```

## Comentários
Comentários, assim como o código, necessitam de manutenção. Isto dá trabalho, e frequentemente pode ser esquecido, deixando-o desatualizado. Por essa razão, evite o uso de comentários tanto quanto possível. O código deve sempre ser auto-explicativo, favorecendo sua legibilidade e clareza. Práticas de código limpo (Clean Code) ajudam a alcançarmos esta meta.

## Exemplo de uso desse guia
```cs
using System;
using UnityEngine;

namespace MyNamespace
{
    public class SampleClass
    {        
        public event Action OnSomethingHappened;
        
        private float _myFloat;
        private float _myOtherfloat;
    
        public SampeClass() { }
        
        public float MyFloat => _myFloat;
    
        public void Public() { }
        
        public int OtherPublic() { }
  
        protected void Protected() { }
  
        private void Private() { }
        
        private void HandleSomethingHappened() { }
    }
}
```
