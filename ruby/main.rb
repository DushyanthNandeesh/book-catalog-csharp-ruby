# frozen_string_literal: true

require_relative 'catalog'
require_relative 'storage'
require_relative 'validation'

# Console menu for the Book Cataloging System (Ruby version).
class BookCatalogApp
  def initialize(path)
    @path = path
    @catalog = load_catalog
  end

  def run
    run_menu
    puts(Storage.save(@path, @catalog) ? 'Catalog saved. Goodbye.' : 'Error: could not save the catalog.')
  end

  private

  # ----- Startup -----

  def load_catalog
    result = Storage.load(@path)
    case result.status
    when :not_found
      puts 'Data file not found. Starting with an empty catalog.'
    when :corrupt
      puts 'Warning: could not read data file. Starting with an empty catalog.'
    end
    Catalog.new(result.books)
  end

  # ----- Menu loop -----

  def run_menu
    loop do
      print_menu
      choice = $stdin.gets
      return if choice.nil? # end of input behaves like "Save and exit"

      case choice.strip
      when '1' then add_book
      when '2' then remove_book
      when '3' then show_books('All Books', @catalog.list_all)
      when '4' then search('title') { |query| @catalog.search_by_title(query) }
      when '5' then search('author') { |query| @catalog.search_by_author(query) }
      when '6' then search('genre') { |query| @catalog.search_by_genre(query) }
      when '7' then show_report('Books by Genre', @catalog.group_by_genre)
      when '8' then show_report('Books by Author', @catalog.group_by_author)
      when '9' then return
      else puts 'Invalid input.'
      end
    end
  end

  def print_menu
    puts
    puts '=== Book Catalog ==='
    puts '1. Add a book'
    puts '2. Remove a book'
    puts '3. List all books'
    puts '4. Search by title'
    puts '5. Search by author'
    puts '6. Search by genre'
    puts '7. Report: books by genre'
    puts '8. Report: books by author'
    puts '9. Save and exit'
    print 'Choose an option: '
  end

  def prompt(label)
    print label
    $stdin.gets.to_s.chomp
  end

  # ----- Menu actions -----

  def add_book
    title = prompt('Title: ')
    author = prompt('Author: ')
    genre = prompt('Genre: ')
    year = Validation.parse_int(prompt('Year: '))

    if [title, author, genre].all? { |text| Validation.valid_text?(text) } && Validation.valid_year?(year)
      @catalog.add(title: title, author: author, genre: genre, year: year)
      puts 'Book added.'
    else
      puts 'Invalid input.'
    end
  end

  def remove_book
    id = Validation.parse_int(prompt('Book ID to remove: '))
    if id.nil?
      puts 'Invalid input.'
    else
      puts(@catalog.remove(id) ? 'Book removed.' : "No book found with ID #{id}.")
    end
  end

  # Runs one of the three searches. The search itself is passed in as a block,
  # so this one method handles title, author, and genre.
  def search(field_name)
    query = prompt("Search #{field_name}: ")
    if Validation.valid_text?(query)
      show_books('Search Results', yield(query))
    else
      puts 'Invalid input.'
    end
  end

  # ----- Output -----

  def show_books(heading, books)
    if books.empty?
      puts 'No books found.'
      return
    end
    puts "=== #{heading} ==="
    books.each { |book| puts book }
  end

  def show_report(heading, groups)
    if groups.empty?
      puts 'No books found.'
      return
    end
    puts "=== #{heading} ==="
    groups.each do |group|
      puts "#{group.name} (#{group.books.size})"
      group.books.each { |book| puts "  #{book}" }
    end
  end
end

# Optional first argument: path to the JSON data file.
default_path = File.expand_path('../data/books.json', __dir__)
BookCatalogApp.new(ARGV[0] || default_path).run
