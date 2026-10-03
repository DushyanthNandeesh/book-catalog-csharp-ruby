# frozen_string_literal: true

# A single book in the catalog.
#
# Ruby feature: no types are declared anywhere. Any object with these five
# methods can be treated like a Book (duck typing), and Ruby only discovers a
# wrong type when the code runs.
class Book
  attr_reader :id, :title, :author, :genre, :year

  def initialize(id:, title:, author:, genre:, year:)
    @id = id
    @title = title
    @author = author
    @genre = genre
    @year = year
  end

  # Shared display format: [id] Title | Author | Genre | Year
  def to_s
    "[#{id}] #{title} | #{author} | #{genre} | #{year}"
  end

  # A Hash with the lowercase keys used in the shared JSON format.
  def to_h
    { id: id, title: title, author: author, genre: genre, year: year }
  end
end

# One group in a report (for example, all books of one genre).
# Ruby feature: Struct builds a small data class in one line.
BookGroup = Struct.new(:name, :books)
