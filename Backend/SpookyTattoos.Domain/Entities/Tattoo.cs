/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

namespace SpookyTattoos.Domain.Entities;

public enum TattooStyles
{
    FLASH, 
    REALISM,
    NEO_TRADITIONAL,
    FINE_LINE,
    MINIMALISM
}

public class Tattoo
{
    public int Id { get; set; }

    public required decimal SizeCm { get; set; }
    
    public string? name {get; set;}
    public required TattooStyles Style { get; set; }

    public decimal FinalTattooPrice { get; set; } = 0;

    public required int JobId { get; set; }
    public Job? Job { get; set; }

    private int _fillScore;
    public int FillScore
    {
        get => _fillScore;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(FillScore), "O Fill Score tem de estar entre 1 e 5.");
            }
            _fillScore = value;
        }
    }

    private int _shadowScore;
    public int ShadowScore
    {
        get => _shadowScore;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(ShadowScore), "O Shadow Score tem de estar entre 1 e 5.");
            }
            _shadowScore = value;
        }
    }

    private int _detailScore;
    public int DetailScore
    {
        get => _detailScore;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(DetailScore), "O Detail Score tem de estar entre 1 e 5.");
            }
            _detailScore = value;
        }
    }

    public bool HasColor { get; set; } = false;

    private int _bodyZoneScore;
    public int BodyZoneScore
    {
        get => _bodyZoneScore;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(BodyZoneScore), "O Body Zone Score tem de estar entre 1 e 5.");
            }
            _bodyZoneScore = value;
        }
    }

    private const decimal BASE_PRICE_FOR_CM = 8.0m;
    private const decimal MINIMUM_PRICE = 40.0m;

    /// <summary>
    /// Calcula o Índice de Complexidade (IC) com base no preenchimento, sombra e detalhe.
    /// Incrementos de 20% por cada nível acima do mínimo.
    /// </summary>
    private decimal CalculateComplexityIndex()
    {
        decimal colorMultiplier = HasColor ? 1.2m : 1.0m;
        
        // Corrigidas as variáveis para os teus novos nomes (Score)
        decimal somaCaracteristicas = FillScore + ShadowScore + DetailScore;
        
        // Fórmula: 1 + ((Soma - 3) / 5)
        decimal baseIndex = 1.0m + ((somaCaracteristicas - 3m) / 5m);
        
        return baseIndex * colorMultiplier;
    }
    /// <summary>
    /// Calcula o multiplicador de dificuldade da zona do corpo (Z).
    /// Incrementos de 10% por cada nível de zona acima de 1.
    /// </summary>
    private decimal CalculateZoneMultiplier()
    {
        // Corrigida a variável para o teu novo nome (BodyZoneScore)
        return 1.0m + ((BodyZoneScore - 1m) / 10m);
    }

    /// <summary>
    /// Estima o preço final da tatuagem, aplicando mínimos e arredondamentos.
    /// </summary>
    public decimal EstimatePrice()
    {
        decimal ic = CalculateComplexityIndex();
        decimal z = CalculateZoneMultiplier();
        
        decimal estimatedRawPrice = SizeCm * BASE_PRICE_FOR_CM * ic * z;

        decimal priceWithMinimum = Math.Max(MINIMUM_PRICE, estimatedRawPrice);

        // 3. Arredondamento para o múltiplo de 5€ mais próximo (por excesso)
        // Exemplo: 41€ / 5 = 8.2 -> Ceiling(8.2) = 9 -> 9 * 5 = 45€
        decimal roundedPrice = Math.Ceiling(priceWithMinimum / 5.0m) * 5.0m;

        return roundedPrice;
    }

    ///<summary>
    /// Com base no valor estimado pela tatuagem, caso o preço praticado da tatuagem for superior a 80€, são atribuidos 10 Ghost Points
    /// Em casos contrários, são atribuidos 5 Ghost Points
    /// </summary>
    public int GhostPoints()
    {
        int ghostPoints = FinalTattooPrice >= 80 ? 10 : 5;
        return ghostPoints;
    }
}