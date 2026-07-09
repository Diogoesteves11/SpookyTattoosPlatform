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
    MINIMALISM
}

public class Tattoo
{
    public int Id {get; set;}

    public required int Size {get; set;}
    public TattooStyles style;

    private int _fillScore;
    public int FillScore
    {
        get => _fillScore;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(FillScore), "The Fill Score should be between 1 and 5.");
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
                throw new ArgumentOutOfRangeException(nameof(ShadowScore), "The Shadow Score should be between 1 and 5.");
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
                throw new ArgumentOutOfRangeException(nameof(DetailScore), "The Detail Score should be between 1 and 5.");
            }
            _detailScore = value;
        }
    }

    public bool HasColor {get; set;} = False;

    private int _bodyZoneScore;
    public int BodyZoneScore
    {
        get => _bodyZoneScore;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(DetailScore), "The Body Zone Score should be between 1 and 5.");
            }
            _bodyZoneScore = value;
        }
    }

    // --- CONSTANTES DE NEGÓCIO ---
    private const decimal PRECO_BASE_POR_CM = 8.0m;
    private const decimal PRECO_MINIMO = 40.0m;

    // --- MÉTODOS DE CÁLCULO DE ORÇAMENTO ---

    /// <summary>
    /// Calcula o Índice de Complexidade (IC) com base no preenchimento, sombra e detalhe.
    /// Incrementos de 20% por cada nível acima do mínimo.
    /// </summary>
    private decimal CalculateComplexityIndex()
    {
        decimal colorMultiplier = HasColor ? 1.2m : 1.0m;
        
        decimal somaCaracteristicas = Fill + Shadow + Detail;
        
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
        return 1.0m + ((BodyZone - 1m) / 10m);
    }

    /// <summary>
    /// Estima o preço final da tatuagem, aplicando mínimos e arredondamentos.
    /// </summary>
    public decimal EstimatePrice()
    {
        decimal ic = CalculateComplexityIndex();
        decimal z = CalculateZoneMultiplier();
        
        decimal estimatedRawPrice = SizeCm * PRECO_BASE_POR_CM * ic * z;

        // 2. Proteção de Margem (Teto Mínimo de 40€)
        decimal priceWithMinimum = Math.Max(PRECO_MINIMO, estimatedRawPrice);

        // 3. Arredondamento para o múltiplo de 5€ mais próximo (Sempre por excesso)
        // Exemplo: 41€ / 5 = 8.2 -> Ceiling(8.2) = 9 -> 9 * 5 = 45€
        decimal roundedPrice = Math.Ceiling(priceWithMinimum / 5.0m) * 5.0m;

        return roundedPrice;
    }

    public decimal GhostPoints()
    {
        return 1;
    }

   
}

