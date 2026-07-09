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


public class PostImage
{
    public string ImageUrl;
    private int _DisplayOrder;
    public int DisplayOrder {
        get => _DisplayOrder;
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(DisplayOrder), "The FDisplay order should be between 1 and 5.");
            }
            _DisplayOrder = value;
        }
    }
}

public class Post
{
    public int Id {get; set;}
    public int JobId {get; set;}

    public Job job {get; set;}

    public string? Description {get; set;}

    public string? PostText {get; set;}

    public DateTimeOffset CreatedAt {get; set;} = DateTimeOffset.UtcNow;

    public bool IsPublished {get; set;} = False;

    ICollection<PostImage> Images = new List<PostImages>();
}