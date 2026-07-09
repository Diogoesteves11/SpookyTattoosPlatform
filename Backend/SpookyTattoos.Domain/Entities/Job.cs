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

public enum JobStatus
{
    AGENDADO,
    EXECUCAO,
    CONCLUIDO,
    CANCELADO,
    REAGENDADO
}

public enum JobType
{
    TATTOO,
    PIERCING
}

public class Job
{
    public int Id { get; set; }

    public required int ClientId { get; set; }

    public Client? Client { get; set; }

    public required JobType Type { get; set; }
    
    public JobStatus Status { get; set; } = JobStatus.AGENDADO; 

    public decimal? FinalPrice { get; set; }
    
    public required DateTimeOffset ScheduledDate { get; set; }

    public required string ReferenceImageUrl {get; set;}


    public ICollection<Tattoo> Tattoos { get; set; } = new List<Tattoo>();
    public ICollection<Piercing> Piercings { get; set; } = new List<Piercing>();

    public TattooCatalogPost? CatalogPost { get; set; }
    public PiercingCatalogPost? PiercingCatalog { get; set; }
}

