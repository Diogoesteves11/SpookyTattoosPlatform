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

using SpookyTattoos.Domain.Entities;

namespace SpookyTattoos.Application.DTOs.Tattoos;

public class CreateTattooDto
{
    public required int JobId { get; set; }
    public required decimal SizeCm { get; set; }
    public string? Name { get; set; }
    public required TattooStyles Style { get; set; }
    
    public required int FillScore { get; set; }
    public required int ShadowScore { get; set; }
    public required int DetailScore { get; set; }
    public required bool HasColor { get; set; }
    public required int BodyZoneScore { get; set; }
}