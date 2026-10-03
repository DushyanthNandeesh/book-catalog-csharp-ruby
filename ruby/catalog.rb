# frozen_string_literal: true

require_relative 'book'
require_relative 'validation'

# The collection of books plus add, remove, search, and report operations.
#
# Ruby features: including Enumerable and defining each gives this class
# select, group_by, sort_by, map, and more for free. Every query below is
# written with blocks.
class Catalog
  include Enumerable

  def initialize(books = [])
    @books = books.dup
  end

  # Required by Enumerable. Passes each book to the caller's block.
  def each(&block)
    return enum_for(:each) unless block_given?

    @books.each(&block)
  end

  def count
    @books.size
  end

  # ----- Add and remove -----

  # Adds a book with the next ID (highest existing ID plus one).
  # Raises ArgumentError if the values break the shared validation rules.
  def add(title:, author:, genre:, year:)
    unless [title, author, genre].all? { |text| Validation.valid_text?(text) } &&
           Validation.valid_year?(year)
      raise ArgumentError, 'Invalid book data.'
    end

    next_id = (map(&:id).max || 0) + 1
    book = Book.new(id: next_id, title: title.strip, author: author.strip,
                    genre: genre.strip, year: year)
    @books << book
    book
  end

  # Removes the book with the given ID. Returns false if there is none.
  def remove(id)
    before = @books.size
    @books.reject! { |book| book.id == id }
    @books.size < before
  end

  # All books ordered by ID.
  def list_all
    sort_by(&:id)
  end

  # ----- Search -----

  def search_by_title(query)
    search(query, &:title)
  end

  def search_by_author(query)
    search(query, &:author)
  end

  def search_by_genre(query)
    search(query, &:genre)
  end

  # ----- Reports -----

  def group_by_genre
    group_books(&:genre)
  end

  def group_by_author
    group_books(&:author)
  end

  private

  # Case-insensitive partial match on one field, sorted by title.
  # The field is chosen by the block (for example &:title), so one method
  # serves all three searches.
  def search(query, &field)
    needle = query.strip.downcase
    select { |book| field.call(book).downcase.include?(needle) }
      .sort_by { |book| [book.title.downcase, book.id] }
  end

  # Groups books (ignoring case), sorts groups alphabetically,
  # and sorts each group's books by year, then title.
  def group_books(&field)
    group_by { |book| field.call(book).downcase }
      .sort_by { |key, _books| key }
      .map do |_key, books|
        BookGroup.new(field.call(books.first),
                      books.sort_by { |book| [book.year, book.title.downcase, book.id] })
      end
  end
end
